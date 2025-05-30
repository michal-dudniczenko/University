(define (problem transport-paczek-problem)
    (:domain transport-paczek)

    (:objects
        truck1 - vehicle
        pkg1 pkg2 pkg3 - package
        locA locB locC - location
    )

    (:init
        (at truck1 locA)
        (at-package pkg1 locA)
        (at-package pkg2 locB)
        (at-package pkg3 locC)
        (capacity-0 truck1)

        (= (travel-time locA locB) 5)
        (= (travel-time locB locA) 5)
        (= (travel-time locB locC) 7)
        (= (travel-time locC locB) 7)
        (= (travel-time locA locC) 10)
        (= (travel-time locC locA) 10)

        (= (total-cost) 0)
    )

    (:goal (and
        (at-package pkg1 locB)
        (at-package pkg2 locC)
        (at-package pkg3 locA)
    ))

    (:metric minimize (total-cost))
)
