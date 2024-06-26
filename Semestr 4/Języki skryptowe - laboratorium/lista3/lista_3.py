import sys
from datetime import datetime


################ zadanie 1 ##########################


def read_log():
    date_format = "%d/%b/%Y:%H:%M:%S"

    result = []

    for line in sys.stdin:
        try:
            tokenized = line.split()

            tokenized[3] = datetime.strptime(tokenized[3][1:], date_format)
            tokenized[-2] = int(tokenized[-2])

            result.append(tuple(tokenized))
        except:
            pass

    return result



def sort_log(tuple_list, sort_nr):
    return sorted(tuple_list, key=lambda x: x[sort_nr])



def get_entries_by_addr(tuple_list, ip_name):
    def validate_ip(ip):
        if " " in ip:
            return False
        
        split_dots = ip.split(".")

        if len(split_dots) != 4:
            return False
        
        for num in split_dots:
            if not num.isalnum():
                return False
        
        return True

    
    def validate_domain_name(domain):
        if " " in domain or len(domain) == 0:
            return False
        return domain.replace('-', '').replace('.', '').isalnum()

    if ip_name.replace(".", "").isdigit():
        if validate_ip(ip_name):
            result = []
            for tup in tuple_list:
                try:
                    if tup[0] == ip_name:
                        result.append(tup)
                except:
                    pass
            return result
        else:
            raise Exception("Invalid ip address or host domain name!")
        
    elif validate_domain_name(ip_name):
        result = []
        for tup in tuple_list:
            try:
                if tup[0] == ip_name:
                    result.append(tup)
            except:
                pass
        return result
    else:
        raise Exception("Invalid ip address or host domain name!")

        
        
def get_failed_reads(tuple_list, shouldMerge):
    list_4xx = []
    list_5xx = []
    
    for tup in tuple_list:
        try:
            if (tup[-2] // 100) == 4:
                list_4xx.append(tup)
            elif (tup[-2] // 100) == 5:
                list_5xx.append(tup)
        except:
            pass

    if shouldMerge:
        return list_4xx + list_5xx
    else:
        return list_4xx, list_5xx



def get_entries_by_extension(tuple_list, extension):
    if extension == "":
        raise Exception("Extension cant be empty string!")
    
    result = []
    
    for tup in tuple_list:
        try:
            if tup[-4].endswith("." + extension):
                 result.append(tup)
        except:
            pass

    return result



def print_entries(tuple_list, number_of_tuples):
    if number_of_tuples >= len(tuple_list):
        number_of_tuples = max(0, len(tuple_list)-1)
    
    for i in range(number_of_tuples):
        print(tuple_list[i])


################ zadanie 2 ##########################
        
def entry_to_dict(tup):
    return {"ip": tup[0], 
            "datetime": tup[3], 
            "time zone": tup[4][:-1], 
            "path": tup[-4], 
            "status code": int(tup[-2]), 
            "size": tup[-1]}


def log_to_dict(tuple_list):
    log_dict = {}

    for tup in tuple_list:
        ip_name = tup[0]
        if ip_name not in log_dict:
            log_dict[ip_name] = []
        log_dict[ip_name].append(entry_to_dict(tup))
    
    return log_dict


def get_addrs(log_dict):
    return list(log_dict.keys())


def print_dict_entry_dates(log_dict):
    for addr, entries in log_dict.items():
        num_requests = len(entries)
        first_request_date = entries[0]["datetime"]
        last_request_date = entries[-1]["datetime"]
        
        print(f"Host: {addr}")
        print(f"Liczba zadan: {num_requests}")
        print(f"Data pierwszego zadania: {first_request_date}")
        print(f"Data ostatniego zadania: {last_request_date}")
        
        status_code_counts = {}
        for entry in entries:
            status_code = entry["status code"]
            status_code_counts[status_code] = status_code_counts.get(status_code, 0) + 1
        for status_code, count in status_code_counts.items():
            ratio = count / num_requests
            print(f"Stosunek liczby zadan z kodem {status_code} do wszystkich: {ratio:.2f}")
        print()


####################################################################################################

def main():
    print("\n\n------------------------------------------------------------------------------\n\n")

    print("\nZadanie 1:\n")
    
    print("Prezentacja read_log():\n")
    
    lista_krotek = read_log()

    print_entries(lista_krotek, 10)
    
    print("\n\n------------------------------------------------------------------------------\n\n")

    print("Prezentacja sort_log(), sortowanie według rozmiaru zasobu:\n")
    print_entries(sort_log(lista_krotek, -1), 10)
    print("\nPrezentacja sort_log(), sortowanie według kodu http:\n")
    print_entries(sort_log(lista_krotek, -2), 10)
    print("\nPrezentacja sort_log(), sortowanie według adresu hosta:\n")
    print_entries(sort_log(lista_krotek, 0), 10)

    print("\n\n------------------------------------------------------------------------------\n\n")

    test_addresses = ["129.94.144.152", "burger.letters.com", "test123 .com", "", "test-123.pl"]

    for addr in test_addresses:
        print(f"\nPrezentacja get_entries_by_addr(lista_krotek, \"{addr}\"):\n")
        try:
            print_entries(get_entries_by_addr(lista_krotek, addr), 10)
        except Exception as e:
            print(e)
    
    print("\n\n------------------------------------------------------------------------------\n\n")
    
    print("Prezentacja get_failed_reads(lista_krotek, shouldMerge=True):\n")
    result_merged = get_failed_reads(lista_krotek, shouldMerge=True)
    print_entries(result_merged, 10)
    print("\nPrezentacja get_failed_reads(lista_krotek, shouldMerge=False):\n")
    result4xx, result5xx = get_failed_reads(lista_krotek, shouldMerge=False)
    print("result 4xx:\n")
    print_entries(result4xx, 10)
    print("\nresult 5xx:\n")
    print_entries(result5xx, 10)

    print("\n\n------------------------------------------------------------------------------\n\n")
    
    test_ext = ["jpg", "png", "123", ""]

    for ext in test_ext:
        print(f"\nPrezentacja get_entries_by_extension(lista_krotek, \"{ext}\"):\n")
        try:
            print_entries(get_entries_by_extension(lista_krotek, ext), 10)
        except Exception as e:
            print(e)

    print("\n\n------------------------------------------------------------------------------\n\n")

    print("\nZadanie 2:\n")

    print("Prezentacja log_to_dict(lista_krotek):\n")

    log_dict = log_to_dict(lista_krotek)

    count = 0
    for key, value in log_dict.items():
        print(f"{key}: {value}")
        count += 1
        if count >= 5:
            break
    
    print("\nPrezentacja get_addrs(log_dict):\n")

    print(get_addrs(log_dict)[:10])

    print("\nPrezentacja print_dict_entry_dates(log_dict):\n")

    print_dict_entry_dates(log_dict)
    
    print("\n\n------------------------------------------------------------------------------\n\n")



if __name__ == "__main__":
    main()
