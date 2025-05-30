(define (domain transport-paczek)
    (:requirements 
        :strips 
        :typing 
        :negative-preconditions 
        :disjunctive-preconditions
        :conditional-effects
        :action-costs 
    )

    (:types
        location
        package
        vehicle
    )

    (:predicates
        (at ?v - vehicle ?l - location)
        (at-package ?p - package ?l - location)
        (in ?p - package ?v - vehicle)
        (capacity-0 ?v - vehicle)
        (capacity-1 ?v - vehicle)
        (capacity-2 ?v - vehicle)
    )

    (:functions
        (total-cost)
        (travel-time ?from - location ?to - location)
    )

    (:action drive
        :parameters (?v - vehicle ?from - location ?to - location)
        :precondition (and
            (at ?v ?from)
            (not (at ?v ?to))
        )
        :effect (and
            (not (at ?v ?from))
            (at ?v ?to)
            (increase (total-cost) (travel-time ?from ?to))
        )
    )

    (:action load
        :parameters (?p - package ?v - vehicle ?l - location)
        :precondition (and
            (at ?v ?l)
            (at-package ?p ?l)
            (not (in ?p ?v))
            (not (capacity-2 ?v))
        )
        :effect (and
            (not (at-package ?p ?l))
            (in ?p ?v)
            (when (capacity-0 ?v)
                (and
                    (not (capacity-0 ?v))
                    (capacity-1 ?v)
                )
            )
            (when (capacity-1 ?v)
                (and
                    (not (capacity-1 ?v))
                    (capacity-2 ?v)
                )
            )
        )
    )
        

    (:action unload
        :parameters (?p - package ?v - vehicle ?l - location)
        :precondition (and
            (at ?v ?l)
            (in ?p ?v)
        )
        :effect (and
            (not (in ?p ?v))
            (at-package ?p ?l)
            (when (capacity-2 ?v)
                (and
                    (not (capacity-2 ?v))
                    (capacity-1 ?v)
                )
            )
            (when (capacity-1 ?v)
                (and
                    (not (capacity-1 ?v))
                    (capacity-0 ?v)
                )
            )
        )
    )
)
