from lista_5 import *
import re
from abc import ABC, abstractmethod



class SSHLogEntry(ABC):
    def __init__(self, entry_log):
        log_dict = get_log_dict(entry_log)
        self.timestamp = log_dict["timestamp"]
        self._raw_log = entry_log   #oznaczenie _raw_log jako niepubliczne
        self.pid = log_dict["pid"]
        self.username = get_user_from_log(log_dict)

    def __str__(self):
        return f"timestamp: {self.timestamp}, pid: {self.pid}, username: {self.username}"
    
    def get_ipv4(self):
        ipv4s = get_ipv4s_from_log(get_log_dict(self._raw_log))

        if ipv4s:
            return ipv4s[0]
        else:
            return None
    
    @abstractmethod
    def validate(self):
        pass

    @property
    def has_ip(self):
        return bool(self.get_ipv4())

    def __repr__(self):
        return f"{self.__class__.__name__}({self._raw_log})"

    def __eq__(self, other):
        return isinstance(other, self.__class__) and self._raw_log == other._raw_log

    def __lt__(self, other):
        return len(other._raw_log) < len(self._raw_log)

    def __gt__(self, other):
        return len(other._raw_log) > len(self._raw_log)
        


class FailedPasswordEntry(SSHLogEntry):
    def __init__(self, entry_log):
        super().__init__(entry_log)
        self.source_ip = self.get_ipv4()
        self.port = self.get_port()

    def __str__(self):
        return super().__str__() + f", source ip: {self.source_ip}, port: {self.port}"
    
    def get_port(self):
        pattern = r"port\s+(\S+)"
        match = re.search(pattern, self._raw_log)
        if match:
            return match.group()
        else:
            return None
    
    def validate(self):
        log_dict = get_log_dict(self._raw_log)
        if not self.timestamp == log_dict["timestamp"]:
            return False
        if not self.pid == log_dict["pid"]:
            return False
        if not self.username == get_user_from_log(get_log_dict(self._raw_log)):
            return False
        if not self.source_ip == self.get_ipv4():
            return False
        if not self.port == self.get_port():
            return False
        return True



class AcceptedPasswordEntry(SSHLogEntry):
    def __init__(self, entry_log):
        super().__init__(entry_log)
        self.source_ip = self.get_ipv4()
        self.port = self.get_port()

    def __str__(self):
        return super().__str__() + f", source ip: {self.source_ip}, port: {self.port}"
    
    def get_port(self):
        pattern = r"port\s+(\S+)"
        match = re.search(pattern, self._raw_log)
        if match:
            return match.group(1)
        else:
            return None
    
    def validate(self):
        log_dict = get_log_dict(self._raw_log)
        if not self.timestamp == log_dict["timestamp"]:
            return False
        if not self.pid == log_dict["pid"]:
            return False
        if not self.username == get_user_from_log(get_log_dict(self._raw_log)):
            return False
        if not self.source_ip == self.get_ipv4():
            return False
        if not self.port == self.get_port():
            return False
        return True




class ErrorEntry(SSHLogEntry):
    def __init__(self, entry_log):
        super().__init__(entry_log)
        self.source_ip = self.get_ipv4()
        self.error_message = self.get_error_message()

    def __str__(self):
        return super().__str__() + f", source ip: {self.source_ip}, error message: {self.error_message}"
    
    def get_error_message(self):
        pattern = r"error:\s+(\S+)"
        match = re.search(pattern, self._raw_log)
        if match:
            return match.group(1)
        else:
            return None
    
    def validate(self):
        log_dict = get_log_dict(self._raw_log)
        if not self.timestamp == log_dict["timestamp"]:
            return False
        if not self.pid == log_dict["pid"]:
            return False
        if not self.username == get_user_from_log(get_log_dict(self._raw_log)):
            return False
        if not self.source_ip == self.get_ipv4():
            return False
        if not self.error_message == self.get_error_message():
            return False
        return True

        


class OtherInfoEntry(SSHLogEntry):
    def __init__(self, entry_log):
        super().__init__(entry_log)
        self.message = self.get_message()

    def __str__(self):
        return super().__str__() + f", message: {self.message}"
    
    def get_message(self):
        pattern = r"\[\d+\]:\s+(.*)"
        match = re.search(pattern, self._raw_log)
        if match:
            return match.group(1)
        else:
            return None
    
    def validate(self):
        return True
