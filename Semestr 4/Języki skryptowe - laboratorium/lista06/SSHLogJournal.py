from zadanie_3_4_5_6 import *
from lista_5 import *



class SSHLogJournal:
    def __init__(self):
        self.entries = []

    def append(self, entry_log):
        log_dict = get_log_dict(entry_log)
        message_type = get_message_type(log_dict["message"])
        if message_type == "udane logowanie":
            entry = AcceptedPasswordEntry(entry_log)
        elif message_type == "nieudane logowanie":
            entry = FailedPasswordEntry(entry_log)
        elif message_type == "błąd":
            entry = ErrorEntry(entry_log)
        else:
            entry = OtherInfoEntry(entry_log)
        
        if entry.validate():
            self.entries.append(entry)

    def __len__(self):
        return len(self.entries)

    def __iter__(self):
        return iter(self.entries)

    def __contains__(self, item):
        return item in self.entries

    def get_logs_by_ip(self, ip):
        filtered_logs = []
        for entry in self.entries:
            if entry.has_ip and entry.get_ipv4() == ip:
                filtered_logs.append(entry)
        return filtered_logs