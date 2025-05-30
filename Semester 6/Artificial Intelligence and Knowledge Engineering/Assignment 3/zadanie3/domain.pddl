(define (domain ball-moving-robot)
    (:requirements 
        :strips 
        :typing 
        :action-costs 
    )

    (:types
        robot
        room
        ball
        arm
    )

    (:predicates
        (at ?r - robot ?rm - room)
        (inroom ?b - ball ?rm - room)
        (holding ?a - arm ?b - ball)
        (arm-empty ?a - arm)
    )

    (:functions
        (total-cost)
    )

    (:action move
        :parameters (?r - robot ?from - room ?to - room)
        :precondition (and
            (at ?r ?from)
        )
        :effect (and
            (not (at ?r ?from))
            (at ?r ?to)
            (increase (total-cost) 1) 
        )
    )

    (:action pick-up
        :parameters (?r - robot ?a - arm ?b - ball ?rm - room)
        :precondition (and
            (at ?r ?rm)
            (inroom ?b ?rm)
            (arm-empty ?a)
        )
        :effect (and
            (holding ?a ?b)
            (not (arm-empty ?a))
            (not (inroom ?b ?rm))
        )
    )

    (:action put-down
        :parameters (?r - robot ?a - arm ?b - ball ?rm - room)
        :precondition (and
            (at ?r ?rm)
            (holding ?a ?b)
        )
        :effect (and
            (inroom ?b ?rm)
            (arm-empty ?a)
            (not (holding ?a ?b))
        )
    )
)
