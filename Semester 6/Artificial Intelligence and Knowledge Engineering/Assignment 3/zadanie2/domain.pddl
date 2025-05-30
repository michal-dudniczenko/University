(define (domain robot-world)
    (:requirements :strips :typing)
    (:types room robot)
    
    (:predicates
        (at ?r - robot ?p - room)
        (connected ?from - room ?to - room)
        (dirty ?p - room)
        (clean ?p - room)
    )
    
    (:action move
        :parameters (?r - robot ?from ?to - room)
        :precondition (and (at ?r ?from) (connected ?from ?to))
        :effect (and (not (at ?r ?from)) (at ?r ?to))
    )

    (:action clean
        :parameters (?r - robot ?p - room)
        :precondition (and (at ?r ?p) (dirty ?p))
        :effect (and (not (dirty ?p)) (clean ?p))
    )
)