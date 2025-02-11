let rec lastElement list =
  match list with
  | [] -> None
  | [x] -> Some x
  | _ :: tail -> lastElement tail



let rec twoLastElements list =
  match list with
  | [] -> None
  | [_] -> None
  | [a; b] -> Some (a, b)
  | _ :: tail -> twoLastElements tail



let rec listLength list =
  match list with
  | [] -> 0
  | _ :: tail -> 1 + listLength tail



let rec reverseList list = 
  match list with
  | [] -> []
  | head :: tail -> reverseList tail @ [head]



let isPalindrome s =
  let rec isPalindromeHelper s =
    if String.length s <= 1 then
      true
    else if s.[0] <> s.[(String.length s) - 1] then
      false
    else
      isPalindromeHelper (String.sub s 1 ((String.length s) - 2))
  in
  let clean_string = String.lowercase_ascii (String.concat "" (String.split_on_char ' ' s)) in
  isPalindromeHelper clean_string



let rec removeDuplicates list = 
  let rec removeAllOcurrences list element = 
    match list with
    | [] -> []
    | head :: tail when head = element -> 
      removeAllOcurrences tail element
    | head :: tail -> head :: removeAllOcurrences tail element
  in
  match list with
  | [] -> []
  | head :: tail -> 
    let filteredTail = removeAllOcurrences tail head in
    head :: removeDuplicates filteredTail



let onlyEvenIndexes list =
  let rec onlyEvenIndexesHelper list index = 
    match list with
    | [] -> []
    | head :: tail when index mod 2 = 0 ->
      head :: onlyEvenIndexesHelper tail (index+1)
    | head :: tail ->
      onlyEvenIndexesHelper tail (index+1)
  in
  onlyEvenIndexesHelper list 0



let isPrime n =
  let rec isPrimeHelper n divisor = 
    if divisor*divisor > n then
      true
    else if n mod divisor = 0 then
      false
    else
      isPrimeHelper n (divisor+2)
  in
  if n <= 1 then
    false
  else if n <= 3 then
    true
  else if n mod 2 = 0 then
    false
  else
    isPrimeHelper n 3

    

let () =
  let list1 = [4; 1; 4; 2; 3; 4; 5; 4; 3; 2; 2; 4] in
  let list2 = [] in
  let list3 = [5] in
  let list4 = [7; 7] in

  Printf.printf "\nLast element:\n";
  Printf.printf "%s\n" (match lastElement list1 with Some x -> string_of_int x | None -> "None");
  Printf.printf "%s\n" (match lastElement list2 with Some x -> string_of_int x | None -> "None");
  Printf.printf "%s\n" (match lastElement list3 with Some x -> string_of_int x | None -> "None");
  Printf.printf "%s\n" (match lastElement list4 with Some x -> string_of_int x | None -> "None");

  Printf.printf "\nTwo last elements:\n";
  Printf.printf "%s\n" (match twoLastElements list1 with Some (a, b) -> Printf.sprintf "(%d, %d)" a b | None -> "None");
  Printf.printf "%s\n" (match twoLastElements list2 with Some (a, b) -> Printf.sprintf "(%d, %d)" a b | None -> "None");
  Printf.printf "%s\n" (match twoLastElements list3 with Some (a, b) -> Printf.sprintf "(%d, %d)" a b | None -> "None");
  Printf.printf "%s\n" (match twoLastElements list4 with Some (a, b) -> Printf.sprintf "(%d, %d)" a b | None -> "None");

  Printf.printf "\nList length:\n";
  Printf.printf "%d\n" (listLength list1);
  Printf.printf "%d\n" (listLength list2);
  Printf.printf "%d\n" (listLength list3);
  Printf.printf "%d\n" (listLength list4);

  Printf.printf "\nReverse list:\n";
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (reverseList list1)));
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (reverseList list2)));
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (reverseList list3)));
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (reverseList list4)));

  Printf.printf "\nIs palindrome:\n";
  Printf.printf "%b\n" (isPalindrome "ka mil slima   k");
  Printf.printf "%b\n" (isPalindrome "test");

  Printf.printf "\nRemove duplicates:\n";
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (removeDuplicates list1)));
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (removeDuplicates list2)));
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (removeDuplicates list3)));
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (removeDuplicates list4)));

  Printf.printf "\nOnly even indexes:\n";
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (onlyEvenIndexes list1)));
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (onlyEvenIndexes list2)));
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (onlyEvenIndexes list3)));
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int (onlyEvenIndexes list4)));

  Printf.printf "\nIs prime:\n";
  Printf.printf "%b\n" (isPrime 47);
  Printf.printf "%b\n" (isPrime 5);
  Printf.printf "%b\n" (isPrime 4);
  Printf.printf "%b\n" (isPrime 25);
  Printf.printf "%b\n" (isPrime 15)
;;