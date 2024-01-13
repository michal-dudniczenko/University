(*zadanie 2*)

let rec f x = f x

type 'a bt = Empty | Node of 'a * 'a bt * 'a bt

(*zadanie 3*)

let breadthBT tree =
  let rec bfs queue acc = match queue with
    | [] -> List.rev acc
    | Empty :: rest -> bfs rest acc
    | Node (value, left, right) :: rest ->
      bfs (rest @ [left; right]) (value :: acc)
  in
  bfs [tree] []
;;

let tt =  Node(1,
            Node(2,
              Node(4,
                Empty,
                Empty
                ),
              Empty
              ),
            Node(3,
              Node(5,
                Empty,
                Node(6,
                  Empty,
                  Empty
                  )
                ),
              Empty
              )
          )
;;

(*zadanie 5*)

type 'a graph = Graph of ('a -> 'a list)

let rec depthSearch graph start =
  let rec dfs visited stack = match stack with
    | [] -> List.rev visited
    | node :: rest ->
      if List.mem node visited then
        dfs visited rest
      else
        let neighbors = match graph with
          | Graph(g) -> g node
        in
        dfs (node :: visited) (neighbors @ rest)
  in
  dfs [] [start]
;;

let g = Graph (fun node ->
  match node with
  | 0 -> [4; 3; 1]
  | 1 -> [0]
  | 2 -> [3]
  | 3 -> [0; 2]
  | 4 -> [0]
  | _ -> []
)
;;

let () =
  Printf.printf "\nZadanie 3: \n";
  let zadanie3 = breadthBT tt in 
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int zadanie3));

  Printf.printf "\nZadanie 5: \n";
  let zadanie5 = depthSearch g 4 in 
  Printf.printf "[%s]\n" (String.concat "; " (List.map string_of_int zadanie5));
;;