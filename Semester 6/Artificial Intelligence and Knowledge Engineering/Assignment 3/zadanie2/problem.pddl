(define (problem robot-clean-problem)
    (:domain robot-world)
    
    (:objects
        pokoj1 pokoj2 pokoj3 - room
        robo - robot
    )
    
    (:init
        (at robo pokoj1)
        (connected pokoj1 pokoj2)
        (connected pokoj2 pokoj1)
        (connected pokoj1 pokoj3)
        (connected pokoj3 pokoj1)
        (dirty pokoj1)
        (dirty pokoj2)
        (dirty pokoj3)
    )
    
    (:goal (and (clean pokoj1) (clean pokoj2) (clean pokoj3)))
)
