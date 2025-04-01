type 'a llist = LNil | LCons of 'a * 'a llist Lazy.t
;;


let rec toLazyList = function
 |  [] -> LNil
 |  x :: xs -> LCons(x, lazy (toLazyList xs))
;;


let rec ltake n llist =
  match (n, llist) with
 |  (0, _) -> []
 |  (_, LNil) -> []
 |  (n, LCons(x, lazy xs)) -> x :: ltake (n-1) xs
;;


let rec lfrom k = LCons (k, lazy (lfrom (k+1)))
;;


(*Zadanie 2*)

let rec lfib =
  let rec fib a b =
    LCons (a, lazy (fib b (a + b)))
  in
  fib 0 1
;;



let () =
  Printf.printf "\nZadanie 2:\n";

  let fibonacci = ltake 10 lfib in 

  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int fibonacci));
;;
