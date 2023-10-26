let flatten1 listOfLists = 
  let rec flatten1Helper result remaining =
    if remaining = [] then result
    else 
      let headList, tailLists = List.hd remaining, List.tl remaining in
      let flatList = result @ headList in
      flatten1Helper flatList tailLists
  in

  flatten1Helper [] listOfLists



let rec count x xs = 
  if xs = [] then 0
  else
    let head,tail = List.hd xs, List.tl xs in 
    if head = x then (1+count x tail) 
    else count x tail



let rec replicate x n =
  if n = 0 then []
  else
    x :: replicate x (n-1)



let rec sqrList xs =
  if xs = [] then []
  else
    let head,tail = List.hd xs, List.tl xs in 
    (head * head) :: sqrList tail 



let rec palindrome xs =
  if List.length xs <= 1 then true
  else 
    if (List.hd xs) = (List.nth xs ((List.length xs )- 1)) then
      palindrome (List.tl (List.rev (List.tl xs)))
    else false



let rec listLength xs =
  if xs = [] then 0
  else
    1 + listLength (List.tl xs)



let () =

Printf.printf "\nflatten1:\n";
let listOfLists = [[5; 6]; [1; 2; 3]; [7; 6; 5; 4; 3; 2; 1]] in
Printf.printf "%s\n" (String.concat " " (List.map string_of_int (flatten1 listOfLists)));

Printf.printf "\ncount:\n";
let list1 = [1; 2; 3; 4; 3; 3; 3; 2; 3] in
Printf.printf "%d\n" (count 3 list1);

Printf.printf "\nreplicate:\n";
Printf.printf "%s\n" (String.concat " " (replicate "test" 5));

Printf.printf "\nsqrList:\n";
let list2 = [2; 3; 4; 5] in
Printf.printf "%s\n" (String.concat " " (List.map string_of_int (sqrList list2)));

Printf.printf "\npalindrome:\n";
Printf.printf "%b\n" (palindrome ["a"; "l"; "a"]);
Printf.printf "%b\n" (palindrome ["t"; "e"; "s"; "t"]);
Printf.printf "%b\n" (palindrome ["k"; "a"; "m"; "i"; "l"; "s"; "l"; "i"; "m"; "a"; "k"]);

Printf.printf "\nlistLength:\n";
let list3 = [1; 2; 3; 4; 5] in
Printf.printf "%d\n" (listLength list3);