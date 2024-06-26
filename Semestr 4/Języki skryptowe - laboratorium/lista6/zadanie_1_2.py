from lista_5 import get_log_dict, get_ipv4s_from_log, get_user_from_log
import re



#nwm tutaj opcjonalna nazwa hosta odnosi sie chyba do username
class SSHLogEntry:
    def __init__(self, entry_log):
        log_dict = get_log_dict(entry_log)
        self.timestamp = log_dict["timestamp"]
        self.raw_log = entry_log
        self.pid = entry_log["pid"]
        self.username = get_user_from_log(entry_log)

    def __str__(self):
        return f"timestamp: {self.timestamp}, pid: {self.pid}, username: {self.username}"
    
    def get_ipv4(self):
        ipv4s = get_ipv4s_from_log(self.raw_log)

        if ipv4s:
            return ipv4s[0]
        else:
            return None
        


class FailedPasswordEntry(SSHLogEntry):
    def __init__(self, entry_log):
        super().__init__(entry_log)
        self.source_ip = self.get_ipv4()
        self.port = self.get_port()

    def __str__(self):
        return super().__str__() + f", source ip: {self.source_ip}, port: {self.port}"
    
    def get_port(self):
        pattern = r"port\s+(\S+)"
        match = re.search(pattern, self.raw_log)
        if match:
            return match.group()
        else:
            return None



class AcceptedPasswordEntry(SSHLogEntry):
    def __init__(self, entry_log):
        super().__init__(entry_log)
        self.source_ip = self.get_ipv4()
        self.port = self.get_port()

    def __str__(self):
        return super().__str__() + f", source ip: {self.source_ip}, port: {self.port}"
    
    def get_port(self):
        pattern = r"port\s+(\S+)"
        match = re.search(pattern, self.raw_log)
        if match:
            return match.group(1)
        else:
            return None



class ErrorEntry(SSHLogEntry):
    def __init__(self, entry_log):
        super().__init__(entry_log)
        self.source_ip = self.get_ipv4()
        self.error_message = self.get_error_message()

    def __str__(self):
        return super().__str__() + f", source ip: {self.source_ip}, error message: {self.error_message}"
    
    def get_error_message(self):
        pattern = r"error:\s+(\S+)"
        match = re.search(pattern, self.raw_log)
        if match:
            return match.group(1)
        else:
            return None
        


class OtherInfoEntry(SSHLogEntry):
    def __init__(self, entry_log):
        super().__init__(entry_log)
        self.message = self.get_message

    def __str__(self):
        return super().__str__() + f", message: {self.message}"
    
    def get_message(self):
        pattern = r"\[\d+\]:\s+(.*)"
        match = re.search(pattern, self.raw_log)
        if match:
            return match.group(1)
        else:
            return None