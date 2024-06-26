from type_annotations import *
from lista_5 import *
import pytest



@pytest.mark.parametrize(
        "raw_log, expected_time",
        [
            ("Dec 10 06:55:46 LabSZ sshd[24200]: Invalid user webmaster from 173.234.31.186", "Dec 10 06:55:46"),
            ("Dec 17 02:08:26 LabSZ sshd[15011]: Connection closed by 183.129.154.138 [preauth]", "Dec 17 02:08:26"),
            ("Dec 31 03:15:21 LabSZ sshd[30365]: Failed password for root from 218.65.30.126 port 31474 ssh2", "Dec 31 03:15:21")
        ]
)
def test_a(raw_log, expected_time):
    entry = SSHLogEntry(raw_log)
    assert entry.timestamp == expected_time



@pytest.mark.parametrize(
        "raw_log, expected_ip",
        [
            ("Dec 10 06:55:48 LabSZ sshd[24200]: Failed password for invalid user webmaster from 173.234.31.186 port 38926 ssh2", "173.234.31.186"),
            ("Dec 10 06:55:48 LabSZ sshd[24200]: Failed password for invalid user webmaster from 666.777.88.213 port 38926 ssh2", "666.777.88.213"),
            ("Dec 10 07:07:38 LabSZ sshd[24206]: pam_unix(sshd:auth): check pass; user unknown", [])
            ]
)
def test_b(raw_log, expected_ip):
    ip = get_ipv4s_from_log(get_log_dict(raw_log))
    if ip:
        ip = ip[0]
    assert ip == expected_ip



@pytest.mark.parametrize(
        "raw_log, expected_type",
        [
            ("Dec 10 09:32:20 LabSZ sshd[24680]: Accepted password for fztu from 119.137.62.142 port 49116 ssh2", AcceptedPasswordEntry),
            ("Dec 10 06:55:48 LabSZ sshd[24200]: Failed password for invalid user webmaster from 173.234.31.186 port 38926 ssh2", FailedPasswordEntry),
            ("Dec 10 09:11:34 LabSZ sshd[24447]: error: Received disconnect from 103.99.0.122: 14: No more user authentication methods available. [preauth]", ErrorEntry),
            ("Dec 10 06:55:46 LabSZ sshd[24200]: pam_unix(sshd:auth): check pass; user unknown", OtherInfoEntry)
        ]
)
def test_c(raw_log, expected_type):
    journal = SSHLogJournal()
    journal.append(raw_log)
    assert isinstance(journal.entries[-1], expected_type)