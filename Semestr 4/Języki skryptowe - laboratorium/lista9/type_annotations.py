from lista_5 import *
from typing import Optional, Iterator
import re



class SSHLogEntry:
    def __init__(self, entry_log: str) -> None:
        log_dict: dict[str, str] = get_log_dict(entry_log)
        self.timestamp: str = log_dict["timestamp"]
        self.raw_log: str = entry_log
        self.pid: str = log_dict["pid"]
        self.username: Optional[str] = get_user_from_log(log_dict)

    def __str__(self) -> str:
        return f"timestamp: {self.timestamp}, pid: {self.pid}, username: {self.username}"
    
    def get_ipv4(self) -> Optional[str]:
        ipv4s: list[str] = get_ipv4s_from_log(get_log_dict(self.raw_log))

        if ipv4s:
            return ipv4s[0]
        else:
            return None
    
    @property
    def has_ip(self) -> bool:
        return bool(self.get_ipv4())
        


class FailedPasswordEntry(SSHLogEntry):
    def __init__(self, entry_log: str) -> None:
        super().__init__(entry_log)
        self.source_ip: Optional[str] = self.get_ipv4()
        self.port: Optional[str] = self.get_port()

    def __str__(self) -> str:
        return super().__str__() + f", source ip: {self.source_ip}, port: {self.port}"
    
    def get_port(self) -> Optional[str]:
        pattern: str = r"port\s+(\S+)"
        match: Optional[re.Match[str]] = re.search(pattern, self.raw_log)
        if match:
            return match.group()
        else:
            return None



class AcceptedPasswordEntry(SSHLogEntry):
    def __init__(self, entry_log: str) -> None:
        super().__init__(entry_log)
        self.source_ip: Optional[str] = self.get_ipv4()
        self.port: Optional[str] = self.get_port()

    def __str__(self) -> str:
        return super().__str__() + f", source ip: {self.source_ip}, port: {self.port}"
    
    def get_port(self) -> Optional[str]:
        pattern: str = r"port\s+(\S+)"
        match: Optional[re.Match[str]] = re.search(pattern, self.raw_log)
        if match:
            return match.group(1)
        else:
            return None



class ErrorEntry(SSHLogEntry):
    def __init__(self, entry_log: str) -> None:
        super().__init__(entry_log)
        self.source_ip: Optional[str] = self.get_ipv4()
        self.error_message: Optional[str] = self.get_error_message()

    def __str__(self) -> str:
        return super().__str__() + f", source ip: {self.source_ip}, error message: {self.error_message}"
    
    def get_error_message(self) -> Optional[str]:
        pattern: str = r"error:\s+(\S+)"
        match: Optional[re.Match[str]] = re.search(pattern, self.raw_log)
        if match:
            return match.group(1)
        else:
            return None
        


class OtherInfoEntry(SSHLogEntry):
    def __init__(self, entry_log: str) -> None:
        super().__init__(entry_log)
        self.message: Optional[str] = self.get_message()

    def __str__(self) -> str:
        return super().__str__() + f", message: {self.message}"
    
    def get_message(self) -> Optional[str]:
        pattern: str = r"\[\d+\]:\s+(.*)"
        match: Optional[re.Match[str]] = re.search(pattern, self.raw_log)
        if match:
            return match.group(1)
        else:
            return None
        


class SSHLogJournal:
    def __init__(self) -> None:
        self.entries: list[SSHLogEntry] = []

    def append(self, entry_log: str) -> None:
        log_dict: dict[str, str] = get_log_dict(entry_log)
        message_type: str = get_message_type(log_dict["message"])

        entry: Optional[SSHLogEntry] = None
        if message_type == "udane logowanie":
            entry = AcceptedPasswordEntry(entry_log)
        elif message_type == "nieudane logowanie":
            entry = FailedPasswordEntry(entry_log)
        elif message_type == "błąd":
            entry = ErrorEntry(entry_log)
        else:
            entry = OtherInfoEntry(entry_log)
        
        self.entries.append(entry)

    def __len__(self) -> int:
        return len(self.entries)

    def __iter__(self) -> Iterator[SSHLogEntry]:
        return iter(self.entries)

    def __contains__(self, item: SSHLogEntry) -> bool:
        return item in self.entries

    def get_logs_by_ip(self, ip: str) -> list[str]:
        filtered_logs: list[str] = []
        for entry in self.entries:
            if entry.has_ip and entry.get_ipv4() == ip:
                filtered_logs.append(entry.raw_log)
        return filtered_logs