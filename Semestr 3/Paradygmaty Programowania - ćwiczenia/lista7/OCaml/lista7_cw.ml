(*Zadanie 1*)

module type QUEUE_FUN = sig
 type 'a t
 exception Empty of string
 val empty: unit -> 'a t
 val enqueue: 'a * 'a t -> 'a t
 val dequeue: 'a t -> 'a t
 val first: 'a t -> 'a
 val isEmpty: 'a t -> bool
end;;

module QueueFunList: QUEUE_FUN = struct
  type 'a t = 'a list 

  exception Empty of string

  let empty () = []

  let enqueue (x, q) = q @ [x]

  let dequeue q =
    match q with
    | [] -> []
    | _ :: tail -> tail
  
  let first q =
    match q with
    | [] -> raise (Empty "List is empty, cant get first")
    | head :: _ -> head
  
  let isEmpty q = q = []
end;;

let test1 () = 
  let open QueueFunList in

  let q1 = empty () in
  let q1 = enqueue (1, q1) in
  let q1 = dequeue q1 in
  let is_empty = isEmpty q1 in
  Printf.printf "Is empty? %b\n" is_empty;
  let q1 = enqueue (2, q1) in
  let q1 = enqueue (3, q1) in
  let element = first q1 in
  Printf.printf "First element: %d\n" element;
  let q1 = dequeue q1 in
  let element = first q1 in
  Printf.printf "First element: %d\n" element;
  let is_empty = isEmpty q1 in
  Printf.printf "Is empty? %b\n" is_empty;
;;

module QueueFunPairList: QUEUE_FUN = struct
  type 'a t = 'a list * 'a list

  exception Empty of string

  let empty () = ([], [])

  let enqueue (x, (front, back)) = 
    let result = (front, x :: back) in 
    match result with 
    | ([], back) -> (List.rev back, [])
    | _ -> result
  
  let dequeue q =
    match q with
    | ([], []) -> ([], [])
    | ([], back) -> (List.tl (List.rev back), [])
    | (head::tail, back) -> 
      let result = (tail, back) in
      match result with 
    | ([], back) -> (List.rev back, [])
    | _ -> result
  
  let first q =
    match q with
    | ([], []) -> raise (Empty "List is empty, cant get first")
    | ([], back) -> List.hd (List.rev back)
    | (head::tail, _) -> head
  
  let isEmpty q = q = ([], [])
end;;

let test2 () = 
  let open QueueFunPairList in

  let q1 = empty () in
  let q1 = enqueue (1, q1) in
  let q1 = dequeue q1 in
  let is_empty = isEmpty q1 in
  Printf.printf "Is empty? %b\n" is_empty;
  let q1 = enqueue (2, q1) in
  let q1 = enqueue (3, q1) in
  let element = first q1 in
  Printf.printf "First element: %d\n" element;
  let q1 = dequeue q1 in
  let element = first q1 in
  Printf.printf "First element: %d\n" element;
  let is_empty = isEmpty q1 in
  Printf.printf "Is empty? %b\n" is_empty;
;;



(*Zadanie 2*)

module type QUEUE_MUT = sig
  type 'a t
  (* The type of queues containing elements of type ['a]. *)
  exception Empty of string
  (* Raised when [first q] is applied to an empty queue [q]. *)
  exception Full of string
  (* Raised when [enqueue(x,q)] is applied to a full queue [q]. *)
  val empty: int -> 'a t
  (* [empty n] returns a new queue of length [n], initially empty. *)
  val enqueue: 'a * 'a t -> unit
  (* [enqueue (x,q)] adds the element [x] at the end of a queue [q]. *)
  val dequeue: 'a t -> unit
  (* [dequeue q] removes the first element in queue [q] *)
  val first: 'a t -> 'a
  (* [first q] returns the first element in queue [q] without removing
  it from the queue, or raises [Empty] if the queue is empty. *)
  val isEmpty: 'a t -> bool
  (* [isEmpty q] returns [true] if queue [q] is empty,
  otherwise returns [false]. *)
  val isFull: 'a t -> bool
  (* [isFull q] returns [true] if queue [q] is full,
  otherwise returns [false]. *)
end;;

module QueueMutable : QUEUE_MUT = struct
  type 'a t = {
    mutable front: int;
    mutable rear: int;
    mutable size: int;
    mutable array: 'a array;
  }

  exception Empty of string
  exception Full of string

  let empty n = { front = 0; rear = 0; size = n; array = Array.make n (Obj.magic 0) }

  let enqueue (x, q) =
    if ((q.rear != q.front) && ((q.rear mod q.size) = (q.front mod q.size))) then
      raise (Full "Array is full, cant enqueue")
    else 
      begin
        q.array.(q.rear mod q.size) <- x;
        q.rear <- q.rear + 1
      end

  let dequeue q =
    if q.front != q.rear then
      q.front <- q.front + 1


  let first q =
    if q.front = q.rear then
      raise (Empty "Array is empty, cant get first")
    else
      q.array.(q.front mod q.size)

  let isEmpty q = q.front = q.rear

  let isFull q = (q.rear != q.front) && ((q.rear mod q.size) = (q.front mod q.size))
end;;

let test3 () = 
  let open QueueMutable in

  let q1 = empty 4 in
  Printf.printf "Is empty? %b\n" (isEmpty q1);
  enqueue (1, q1);
  enqueue (2, q1);
  enqueue (3, q1);
  Printf.printf "First element: %d\n" (first q1);
  enqueue (4, q1);
  dequeue q1;
  dequeue q1;
  Printf.printf "First element: %d\n" (first q1);
  Printf.printf "Is empty? %b\n" (isEmpty q1);
;;



(*Testy*)

let () = 
  Printf.printf "\nSingle list:\n";
  test1 ();

  Printf.printf "\nPair of lists:\n";
  test2 ();

  Printf.printf "\nMutable array:\n";
  test3 ();
;;