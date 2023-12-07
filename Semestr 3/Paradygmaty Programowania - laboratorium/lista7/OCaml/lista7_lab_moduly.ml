(*Zadanie 1*)

module type Point3DType = sig
    type t
    val create : float -> float -> float -> t
    val distance : t -> t -> float
end

module Point3D : Point3DType = struct
  type t = {x: float; y: float; z: float}

  let create x y z = {x; y; z}

  let distance p1 p2 =
    let dx = p1.x -. p2.x in
    let dy = p1.y -. p2.y in
    let dz = p1.z -. p2.z in
    sqrt (dx *. dx +. dy *. dy +. dz *. dz)

end


(*Zadanie 2*)

module type SegmentType = sig
    type t
    val create : Point3D.t -> Point3D.t -> t
    val length : t -> float
end

module Segment: SegmentType = struct
    type t = {start_point: Point3D.t; end_point: Point3D.t}
    let create start_point end_point = {start_point; end_point}
    let length segm = Point3D.distance segm.start_point segm.end_point
end


(*Zadanie 3*)

module type BinaryTreeType = sig
    type 'a tree
    val empty : 'a tree
    val add : 'a -> 'a tree -> 'a tree
    val remove : 'a -> 'a tree -> 'a tree
    val preorder : 'a tree -> 'a list
    val inorder : 'a tree -> 'a list
    val postorder : 'a tree -> 'a list
    val leaves : 'a tree -> 'a list
  end
  
module BinaryTree : BinaryTreeType = struct
    type 'a tree = Leaf | Node of 'a * 'a tree * 'a tree

    let empty = Leaf

    let rec add x tree =
        match tree with
        | Leaf -> Node (x, Leaf, Leaf)
        | Node (value, left, right) ->
        if x < value then
            Node (value, add x left, right)
        else if x > value then
            Node (value, left, add x right)
        else
            tree

    let rec remove x tree =
        match tree with
        | Leaf -> Leaf
        | Node (value, left, right) ->
        if x < value then
            Node (value, remove x left, right)
        else if x > value then
            Node (value, left, remove x right)
        else
            match (left, right) with
            | (Leaf, _) -> right
            | (_, Leaf) -> left
            | (_, _ )->
            let min_right = find_min right in
            Node (min_right, left, remove min_right right)

    and find_min tree = match tree with
        | Leaf -> failwith "Empty tree"
        | Node (value, Leaf, _) -> value
        | Node (_, left, _) -> find_min left

    let rec preorder tree =
        match tree with
        | Leaf -> []
        | Node (value, left, right) ->
        [value] @ (preorder left) @ (preorder right)

    let rec inorder tree =
        match tree with
        | Leaf -> []
        | Node (value, left, right) ->
        (inorder left) @ [value] @ (inorder right)

    let rec postorder tree =
        match tree with
        | Leaf -> []
        | Node (value, left, right) ->
        (postorder left) @ (postorder right) @ [value]

    let rec leaves tree =
        match tree with
        | Leaf -> []
        | Node (value, Leaf, Leaf) -> [value]
        | Node (_, left, right) -> (leaves left) @ (leaves right)
end
  