import re
from datetime import datetime



def get_log_dict(log_line):
    pattern = r"(\S+).*?\[(\S+?):(\S+)\s+(\S+?)\]\s+\"(\S+)\s+(\S+).*?\"\s+(\d+)\s+(.*)"
    match = re.search(pattern, log_line)
    return {
        "host": match.group(1),
        "date": datetime.strptime(match.group(2), "%d/%b/%Y").strftime("%d.%m.%Y"),
        "time": match.group(3),
        "timezone": f"UTC {int(match.group(4)[:3])} hours",
        "method": match.group(5),
        "resource": match.group(6),
        "status": match.group(7),
        "size": match.group(8) + " B"
    }



def check_if_valid(log_line):
    pattern = r"(\S+).*?\[(\S+?):(\S+)\s+(\S+?)\]\s+\"(\S+)\s+(\S+).*?\"\s+(\d+)\s+(.*)"
    match = re.search(pattern, log_line)
    return match != None



def filter_date(log, date1, date2):
    pattern = r"\[(\S+?):"
    match = re.search(pattern, log)
    date = datetime.strptime(match.group(1), "%d/%b/%Y").strftime("%d.%m.%Y")
    
    date1 = datetime.strptime(date1, "%d.%m.%Y").date()
    date2 = datetime.strptime(date2, "%d.%m.%Y").date()
    date = datetime.strptime(date, "%d.%m.%Y").date()

    return date1 <= date <= date2





if __name__ == "__main__":
    counter = 100
    with open("NASA", encoding="utf-8") as file:
        codes = []
        for line in file:
            try:
                log_dict = get_log_dict(line)
                status = log_dict["status"]
                if status not in codes:
                    codes.append(status)
            except:
                pass
        
        print(codes)