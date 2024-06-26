import re
import logging
import sys
import random



#zadanie 1.1
def get_log_dict(log_entry):
    log_pattern = r"(\S+\s+\d+\s+\d+:\d+:\d+)\s+(\S+)\s+(\S+)\[(\d+)\]:\s+(.*)"

    timestamp, hostname, component, pid, message = re.match(log_pattern, log_entry).groups()

    return {
        "timestamp": timestamp,
        "hostname": hostname,
        "component": component,
        "pid": pid,
        "message": message
    }



#zadanie 1.2
def get_ipv4s_from_log(entry_dict):
    message = entry_dict["message"]

    ipv4_pattern = r"\b(?:[0-9]{1,3}\.){3}[0-9]{1,3}\b"

    ipv4s = re.findall(ipv4_pattern, message)

    return ipv4s



#zadanie 1.3
def get_user_from_log(entry_dict):
    message = entry_dict["message"].lower()

    patterns = [
        r"user[ =]{1}(\S+)",
        r"for (\S+) from",
        r"getaddrinfo for (\S+)"
    ]

    for pattern in patterns:
        match = re.search(pattern, message)
        if match:
            return match.group(1)

    return None



#zadanie 1.4
def get_message_type(message):
    message = message.lower()

    if re.search(r"accepted password for", message):
        return "udane logowanie"
    elif re.search(r"failed password for", message):
        return "nieudane logowanie"
    elif re.search(r"error", message):
        return "błąd"
    elif re.search(r"connection closed", message) or re.search(r"received disconnect", message):
        return "zamknięcie połączenia"
    elif re.search(r"reverse mapping checking getaddrinfo", message):
        return "próba włamania"
    elif re.search(r"invalid user", message.lower()):
        return "błędna nazwa użytkownika"
    elif re.search(r"pam_unix\(sshd:auth\): check pass", message):
        return "błędne hasło"
    else:
        return "inne"



#zadanie 2 helper
class MaxLevelFilter(logging.Filter):
    def __init__(self, max_level):
        self.max_level = max_level
        logging.Filter.__init__(self)
    def filter(self, record):
        if record.levelno <= self.max_level:
            return True
        return False
    


#zadanie 3
def get_user_log_dict():
    user_logs = {}

    with open("SSH.log") as log_file:
        for line in log_file:
            log_dict = get_log_dict(line)

            user = get_user_from_log(log_dict)
            if user:
                if user in user_logs.keys():
                    user_logs[user].append(line)
                else:
                    user_logs[user] = [line]

    return user_logs



def get_random_logs(number_of_logs):
    result = []
    if number_of_logs <= 0:
        return result

    user_logs = get_user_log_dict()
    
    random_user = random.choice(list(user_logs.keys()))

    number_of_logs = min(number_of_logs, len(user_logs[random_user]))

    for _ in range(number_of_logs):
        random_choice = random.randint(0, len(user_logs[random_user]) - 1)
        result.append(user_logs[random_user][random_choice])
        user_logs[random_user].pop(random_choice)
    
    return result



def least_most_frequent_users():
    user_logs = get_user_log_dict()

    least_freq_user = ""
    least_freq_logins = sys.maxsize

    most_freq_user = ""
    most_freq_logins = 0

    for user, logs in user_logs.items():
        login_count = 0

        for log in logs:
            if get_message_type(log) == "udane logowanie":
                login_count += 1
        
        if login_count < least_freq_logins and login_count > 0:
            least_freq_user = user
            least_freq_logins = login_count
        
        if login_count > most_freq_logins:
            most_freq_user = user
            most_freq_logins = login_count
    
    return least_freq_user, least_freq_logins, most_freq_user, most_freq_logins



def main():
    print("\n-------------------------------------------------------\n")

    #zadanie 2
    logger = logging.getLogger("logger")
    logger.setLevel(logging.DEBUG)
    stdout_formatter = logging.Formatter("stdout: %(asctime)s - %(levelname)s - %(message)s")
    stderr_formatter = logging.Formatter("stderr: %(asctime)s - %(levelname)s - %(message)s")

    stdout_handler = logging.StreamHandler(sys.stdout)
    stdout_handler.setLevel(logging.DEBUG)
    stdout_handler.setFormatter(stdout_formatter)
    stdout_handler.addFilter(MaxLevelFilter(30))
    logger.addHandler(stdout_handler)

    stderr_handler = logging.StreamHandler(sys.stderr)
    stderr_handler.setLevel(logging.ERROR)
    stderr_handler.setFormatter(stderr_formatter)
    logger.addHandler(stderr_handler)


    lines_read = 0
    with open("SSH.log") as log_file:
        for line in log_file:
            logger.debug(f"bytes read: {len(line)}")

            entry_dict = get_log_dict(line)

            message_type = get_message_type(entry_dict["message"])

            if message_type == "udane logowanie" or message_type == "zamknięcie połączenia":
                logger.info(entry_dict["message"])
            elif message_type == "nieudane logowanie":
                logger.warning(entry_dict["message"])
            elif message_type == "błędna nazwa użytkownika" or message_type == "błędne hasło" or re.search(r"error", entry_dict["message"]):
                logger.error(entry_dict["message"])
            elif message_type == "próba włamania":
                logger.critical(entry_dict["message"])

            print("log: " + line.rstrip())
            print(f"ipv4s: {get_ipv4s_from_log(entry_dict)}")
            print(f"user: {get_user_from_log(entry_dict)}")
            print(f"message type: {message_type}")
            print()

            lines_read += 1
            if lines_read == 10:
                break
    
    
    print("\n\nn losowo wybranych wpisów z logami dotyczących losowo wybranego użytkownika, n = 3")
    for log in get_random_logs(3):
        print(log.rstrip())

    print("\n\nn losowo wybranych wpisów z logami dotyczących losowo wybranego użytkownika, n = 100")
    for log in get_random_logs(100):
        print(log.rstrip())

    print("\n\nn losowo wybranych wpisów z logami dotyczących losowo wybranego użytkownika, n = -1")
    print(get_random_logs(-1))


    least_freq_user, least_freq_logins, most_freq_user, most_freq_logins = least_most_frequent_users()

    print(f"\nUżytkownik, który logował się najrzadziej: {least_freq_user}, logował się: {least_freq_logins} razy")
    print(f"Użytkownik, który logował się najczęsciej: {most_freq_user}, logował się: {most_freq_logins} razy")
    


if __name__ == "__main__":
    main()