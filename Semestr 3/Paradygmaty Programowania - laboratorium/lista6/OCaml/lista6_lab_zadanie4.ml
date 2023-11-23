type 'a sequence = Nil | Cons of 'a * (unit -> 'a sequence)
let rec from n = Cons (n, fun () -> from (n + 1))



let rec stirling n k = 
  if (k = 1) || (k = n) then 1
  else if (k = 0) || (k > n) then 0
  else stirling (n-1) (k-1) + k * stirling (n-1) k



let bell_number n = 
  let rec bell_number_helper acc k =
    if k > n then acc
    else bell_number_helper (stirling n k + acc) (k+1)
  in 

  bell_number_helper 0 0



let rec bell_number_stream n = 
  Cons(bell_number n, fun() -> bell_number_stream (n+1))



let stream_head stream =
  match stream with
  | Cons (x, _) -> x
  | Nil -> failwith "Stream is empty!"



let stream_tail stream =
  match stream with
  | Cons (_, tail) -> tail
  | Nil -> failwith "Stream is empty!"



let rec take n stream =
  if n = 0 then []
  else 
    match stream with
    | Nil -> []
    | Cons(head, tail) -> head :: take (n-1) (tail ()) 



let take_even n stream =
  let rec take_even_helper n stream test =
    if n = 0 then []
    else if test mod 2 = 0 then
      match stream with
      | Nil -> []
      | Cons(head, tail) -> head :: take_even_helper (n-1) (tail ()) (test + 1)
    else 
      match stream with
      | Nil -> []
      | Cons(_, tail) -> take_even_helper n (tail ()) (test + 1)
    
  in  
  
  take_even_helper n stream 0



let rec drop n stream =
  if n = 0 then stream 
  else 
    match stream with
    | Nil -> Nil 
    | Cons(_, tail) -> drop (n-1) (tail ())



let rec pair_streams n stream1 stream2 = 
  if n = 0 then []
  else
    match (stream1, stream2) with
    | (Cons(head1, tail1), Cons(head2, tail2)) -> (head1, head2) :: pair_streams (n-1) (tail1()) (tail2())
    | _ -> []



let rec map_stream f stream =
  match stream with
  | Nil -> Nil
  | Cons (head, tail) -> Cons (f head, fun () -> map_stream f (tail ()))



let () =
  let natural = from 0 in
  let bell = bell_number_stream 0 in 

  let first_elements = take 5 bell in 
  Printf.printf "First n elements: %s\n" (String.concat ", " (List.map string_of_int first_elements));

  let first_even_elements = take_even 5 bell in 
  Printf.printf "First n even elements: %s\n" (String.concat ", " (List.map string_of_int first_even_elements));

  let skip_elements = drop 2 bell in
  let first_elements_skipped = take 5 skip_elements in 
  Printf.printf "First n elements after skipping some: %s\n" (String.concat ", " (List.map string_of_int first_elements_skipped));

  let paired = pair_streams 5 natural bell in
  Printf.printf "First n paired elements from natural and bell: %s\n" (String.concat " " (List.map (fun (x, y) -> Printf.sprintf "(%d, %d)" x y) paired));

  let mapped = map_stream (fun x -> x * x) natural in 
  let first_elements_mapped = take 5 mapped in 
  Printf.printf "First n elements of mapped stream: %s\n" (String.concat ", " (List.map string_of_int first_elements_mapped));