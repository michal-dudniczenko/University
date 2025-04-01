let rec evenR n = 
  Printf.printf "evenR(%d)\n" n;
  if n = 0 then true
  else oddR (n-1)

  and oddR n =
    Printf.printf "oddR(%d)\n" n;
    if n = 0 then false
    else evenR (n-1)



let rec fib n =
  if n = 0 then 0
  else if n = 1 then 1
  else fib (n-2) + fib (n-1) 



let fibTail n =
  let rec fibTailHelper n a b =
    if n = 0 then a
    else fibTailHelper (n-1) b (a+b)
  in 

  fibTailHelper n 0 1



let root3 a =
  let epsilon = 1e-15 in
  let rec root3Helper x =
    if abs_float ((x ** 3.0) -. a) <= epsilon *. abs_float a then x
    else 
      let nextX = x +. (a /. (x *. x) -. x) /. 3.0 in 
      root3Helper nextX
    in

    if a > 1.0 then 
      root3Helper (a /. 3.0)
    else 
      root3Helper a



let rec initSegment xs ys =
  match (xs, ys) with
  | [],_ -> true
  | x :: xRest, y :: yRest when x = y -> initSegment xRest yRest
  | _ -> false



let replaceNth xs n x =
  let rec replaceNthHelper xs n =
    match xs with
    | head :: tail when n = 0 -> x :: tail
    | head :: tail -> head :: replaceNthHelper tail (n-1)
    | []  -> []
  in 

  if n < 0 || n >= List.length xs then
    failwith "Index out of range"
  else
    replaceNthHelper xs n 



let () =
  Printf.printf "\nzadanie 1:\n";
  ignore (evenR 3);

  Printf.printf "\nzadanie 2:\n";
  (* Printf.printf "%d\n" (fib 42); *)
  Printf.printf "%d\n" (fibTail 42);

  Printf.printf "\nzadanie 3:\n";
  Printf.printf "%f\n" (root3 76.0);

  Printf.printf "\nzadanie 4:\n";
  let (_, _, x, _, _) = (-2, -1, 0, 1, 2) in
  (* let (_, (x, _)) = ((1, 2), (0, 1)) in *)
  Printf.printf "%d\n" x;

  Printf.printf "\nzadanie 5:\n";
  Printf.printf "%b\n" (initSegment [1; 2; 3] [1; 2; 3; 4; 5]);

  Printf.printf "\nzadanie 6:\n";
  List.iter (fun item -> Printf.printf "%d " item) (replaceNth [1; 2; 3; 4; 5; 6] 2 777);
  Printf.printf "\n";