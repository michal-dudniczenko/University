(*Zadanie 4*)

module type CoordinateType = sig
  type t
end

module IntCoordinate : CoordinateType = struct
  type t = int
end

module FloatCoordinate : CoordinateType = struct
  type t = float
end

module MakePoint (Coordinate : CoordinateType) = struct
  type t = { x: Coordinate.t; y: Coordinate.t; z: Coordinate.t }
  let create x y z = {x; y; z}
  let get_x point = point.x
  let get_y point = point.y
  let get_z point = point.z
end


(*Zadanie 5*)

module type Translation = sig
  val x: float
  val y: float
  val z: float
end

module Translation_1_1_1: Translation = struct
  let x = 1.0
  let y = 1.0
  let z = 1.0
end


(*Zadanie 6*)

module type PointType = sig
  type t
  val create : float -> float -> float -> t
  val get_x : t -> float
  val get_y : t -> float
  val get_z : t -> float
end

module Point3D: PointType = struct
  type t = {x: float; y: float; z: float}
  let create x y z = {x; y; z}
  let get_x point = point.x
  let get_y point = point.y
  let get_z point = point.z
end

module type SegmentType = sig
  type t
  val create : Point3D.t -> Point3D.t -> t
  val get_start_point : t -> Point3D.t
  val get_end_point : t -> Point3D.t
end

module Segment: SegmentType = struct
  type t = {start_point: Point3D.t; end_point: Point3D.t}
  let create start_point end_point = { start_point; end_point }
  let get_start_point segment = segment.start_point
  let get_end_point segment = segment.end_point
end

module Translate_Point (Point: PointType) (Vector: Translation): PointType = struct
  type t = Point.t
  let create x y z = Point.create (x +. Vector.x) (y +. Vector.y) (z +. Vector.z)
  let get_x point = (Point.get_x point) +. Vector.x
  let get_y point = (Point.get_y point) +. Vector.y
  let get_z point = (Point.get_z point) +. Vector.z
end

module Translate_Segment (Segment: SegmentType) (Vector: Translation): SegmentType = struct
  type t = Segment.t
  
  let create start_point end_point =
    let updated_start_point =
      Point3D.create
        (Point3D.get_x start_point +. Vector.x)
        (Point3D.get_y start_point +. Vector.y)
        (Point3D.get_z start_point +. Vector.z)
    in
    let updated_end_point =
      Point3D.create
        (Point3D.get_x end_point +. Vector.x)
        (Point3D.get_y end_point +. Vector.y)
        (Point3D.get_z end_point +. Vector.z)
    in
    Segment.create updated_start_point updated_end_point  
  
  let get_start_point segment = 
    let old_start_point = Segment.get_start_point segment in
    let updated_start_point =
      Point3D.create
        (Point3D.get_x old_start_point +. Vector.x)
        (Point3D.get_y old_start_point +. Vector.y)
        (Point3D.get_z old_start_point +. Vector.z)
    in
    updated_start_point

  let get_end_point segment = 
    let old_end_point = Segment.get_end_point segment in
    let updated_end_point =
      Point3D.create
        (Point3D.get_x old_end_point +. Vector.x)
        (Point3D.get_y old_end_point +. Vector.y)
        (Point3D.get_z old_end_point +. Vector.z)
    in
    updated_end_point
end
