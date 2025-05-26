package com.example.befit.constants

import com.example.befit.R
import com.example.befit.database.Exercise

val DEFAULT_EXERCISES = listOf(
    Exercise(
        name = "Bench press",
        videoId = R.raw.bench_press
    ),
    Exercise(
        name = "Deadlift",
        videoId = R.raw.deadlift
    ),
    Exercise(
        name = "Squat",
        videoId = R.raw.squat
    ),
    Exercise(
        name = "Overhead press",
        videoId = R.raw.overhead_press
    ),
    Exercise(
        name = "Pull-up",
        videoId = R.raw.pull_up
    ),
    Exercise(
        name = "Incline dumbbell curl",
        videoId = R.raw.incline_dumbbell_curl
    )
)