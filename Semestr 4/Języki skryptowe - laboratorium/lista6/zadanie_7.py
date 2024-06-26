import re
from SSHLogJournal import SSHLogJournal
from lista_5 import *



class SSHUser:
    def __init__(self, username, last_login_date="01.01.2001"):
        self.username = username
        self.last_login_date = last_login_date
    
    def __str__(self):
        return f"username: {self.username}, last login date: {self.last_login_date}"

    def validate(self):
        pattern =  r"^[a-z_][a-z0-9_-]{0,31}$"
        if re.match(pattern, self.username):
            return True
        else:
            return False
    


def duck_typing_test():
    users_and_logs = []

    user1 = SSHUser("michal2001")
    users_and_logs.append(user1)
    user2 = SSHUser("_michal2001___")
    users_and_logs.append(user2)
    user3 = SSHUser("za_dlugi_username_48976219874619984621985")
    users_and_logs.append(user3)
    user4 = SSHUser("x")
    users_and_logs.append(user4)
    user5 = SSHUser("niepoprawne_znaki_%^&")
    users_and_logs.append(user5)
    user6 = SSHUser("!username")
    users_and_logs.append(user6)

    journal = SSHLogJournal()

    with open("SSH.log") as log_file:
        for line in log_file:
            journal.append(line.rstrip())
    
    logs_limit = 10

    for log in journal:
        users_and_logs.append(log)
        logs_limit -= 1
        if logs_limit == 0:
            break


    for obj in users_and_logs:
        print(str(obj) + f"\t\t\tvalidate: {obj.validate()}")



if __name__ == "__main__":
    duck_typing_test()