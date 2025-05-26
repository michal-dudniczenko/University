package com.example.befit.trainingprograms

import androidx.compose.animation.EnterTransition
import androidx.compose.animation.ExitTransition
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.example.befit.constants.TrainingProgramsRoutes
import com.example.befit.trainingprograms.settings.AddExerciseScreen
import com.example.befit.trainingprograms.settings.EditExerciseListScreen
import com.example.befit.trainingprograms.settings.EditExerciseScreen
import com.example.befit.trainingprograms.settings.SettingsScreen

@Composable
fun TrainingProgramsNavigation(
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val navController = rememberNavController()
    val currentRoute by viewModel.currentRoute

    LaunchedEffect(Unit) {
        viewModel.navigationEvent.collect { route ->
            navController.navigate(route)
        }
    }

    NavHost(
        navController = navController,
        startDestination = currentRoute,
        enterTransition = { EnterTransition.None },
        exitTransition = { ExitTransition.None },
        modifier = modifier
            .fillMaxSize()
    ) {
        composable(route = TrainingProgramsRoutes.PROGRAMS_LIST) {
            viewModel.updateCurrentRoute(TrainingProgramsRoutes.PROGRAMS_LIST)
            ProgramsListScreen(
                viewModel = viewModel,
                navController = navController
            )
        }
        composable(route = TrainingProgramsRoutes.ADD_PROGRAM) {
            viewModel.updateCurrentRoute(TrainingProgramsRoutes.ADD_PROGRAM)
            AddProgramScreen(
                viewModel = viewModel,
                navController = navController
            )
        }
        composable(
            route = TrainingProgramsRoutes.EDIT_PROGRAM,
            arguments = listOf(navArgument("id") { type = NavType.IntType })
        ) { backStackEntry ->
            val programId = backStackEntry.arguments?.getInt("id")
            programId?.let {
                viewModel.updateCurrentRoute(TrainingProgramsRoutes.EDIT_PROGRAM(programId))
                EditProgramScreen(
                    programId = it,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = TrainingProgramsRoutes.TRAINING_DAYS_LIST,
            arguments = listOf(navArgument("id") { type = NavType.IntType })
        ) { backStackEntry ->
            val programId = backStackEntry.arguments?.getInt("id")
            programId?.let {
                viewModel.updateCurrentRoute(TrainingProgramsRoutes.TRAINING_DAYS_LIST(programId))
                TrainingDaysListScreen(
                    programId = it,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = TrainingProgramsRoutes.ADD_TRAINING_DAY,
            arguments = listOf(navArgument("programId") { type = NavType.IntType })
        ) { backStackEntry ->
            val programId = backStackEntry.arguments?.getInt("programId")
            programId?.let {
                viewModel.updateCurrentRoute(TrainingProgramsRoutes.ADD_TRAINING_DAY(programId))
                AddTrainingDayScreen(
                    programId = it,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = TrainingProgramsRoutes.EDIT_TRAINING_DAY,
            arguments = listOf(
                navArgument("id") { type = NavType.IntType },
            )
        ) { backStackEntry ->
            val trainingDayId = backStackEntry.arguments?.getInt("id")
            if (trainingDayId != null) {
                viewModel.updateCurrentRoute(TrainingProgramsRoutes.EDIT_TRAINING_DAY(trainingDayId))
                EditTrainingDayScreen(
                    trainingDayId = trainingDayId,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = TrainingProgramsRoutes.VIEW_TRAINING_DAY,
            arguments = listOf(
                navArgument("id") { type = NavType.IntType }
            )
        ) { backStackEntry ->
            val trainingDayId = backStackEntry.arguments?.getInt("id")
            if (trainingDayId != null) {
                viewModel.updateCurrentRoute(TrainingProgramsRoutes.VIEW_TRAINING_DAY(trainingDayId))
                ViewTrainingDayScreen(
                    trainingDayId = trainingDayId,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = TrainingProgramsRoutes.ADD_EXERCISE_TO_DAY,
            arguments = listOf(
                navArgument("trainingDayId") { type = NavType.IntType },
            )
        ) { backStackEntry ->
            val trainingDayId = backStackEntry.arguments?.getInt("trainingDayId")

            if (trainingDayId != null) {
                viewModel.updateCurrentRoute(TrainingProgramsRoutes.ADD_EXERCISE_TO_DAY(trainingDayId))
                AddExerciseToDayScreen(
                    trainingDayId = trainingDayId,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = TrainingProgramsRoutes.EDIT_EXERCISE_FROM_DAY,
            arguments = listOf(
                navArgument("id") { type = NavType.IntType },
            )
        ) { backStackEntry ->
            val trainingDayExerciseId = backStackEntry.arguments?.getInt("id")

            if (trainingDayExerciseId != null) {
                viewModel.updateCurrentRoute(TrainingProgramsRoutes.EDIT_EXERCISE_FROM_DAY(trainingDayExerciseId))
                EditExerciseFromDayScreen(
                    trainingDayExerciseId = trainingDayExerciseId,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = TrainingProgramsRoutes.VIEW_EXERCISE_FROM_DAY,
            arguments = listOf(
                navArgument("id") { type = NavType.IntType },
            )
        ) { backStackEntry ->
            val trainingDayExerciseId = backStackEntry.arguments?.getInt("id")

            if (trainingDayExerciseId != null) {
                viewModel.updateCurrentRoute(TrainingProgramsRoutes.VIEW_EXERCISE_FROM_DAY(trainingDayExerciseId))
                ViewExerciseFromDayScreen(
                    trainingDayExerciseId = trainingDayExerciseId,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(route = TrainingProgramsRoutes.SETTINGS) {
            viewModel.updateCurrentRoute(TrainingProgramsRoutes.SETTINGS)
            SettingsScreen(
                navController = navController
            )
        }
        composable(route = TrainingProgramsRoutes.EDIT_EXERCISE_LIST) {
            viewModel.updateCurrentRoute(TrainingProgramsRoutes.EDIT_EXERCISE_LIST)
            EditExerciseListScreen(
                navController = navController,
                trainingProgramsViewModel = viewModel
            )
        }
        composable(route = TrainingProgramsRoutes.ADD_EXERCISE) {
            viewModel.updateCurrentRoute(TrainingProgramsRoutes.ADD_EXERCISE)
            AddExerciseScreen(
                navController = navController,
                viewModel = viewModel
            )
        }
        composable(
            route = TrainingProgramsRoutes.EDIT_EXERCISE,
            arguments = listOf(navArgument("id") { type = NavType.IntType })
        ) { backStackEntry ->
            val exerciseId = backStackEntry.arguments?.getInt("id")
            exerciseId?.let {
                viewModel.updateCurrentRoute(TrainingProgramsRoutes.EDIT_EXERCISE(exerciseId))
                EditExerciseScreen(
                    exerciseId = it,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = TrainingProgramsRoutes.EXERCISE_VIDEO,
            arguments = listOf(
                navArgument("exerciseId") { type = NavType.IntType },
                navArgument("trainingDayExerciseId") { type = NavType.IntType }
            )
        ) { backStackEntry ->
            val exerciseId = backStackEntry.arguments?.getInt("exerciseId")
            val trainingDayExerciseId = backStackEntry.arguments?.getInt("trainingDayExerciseId")

            if (exerciseId != null && trainingDayExerciseId != null) {
                viewModel.updateCurrentRoute(TrainingProgramsRoutes.EXERCISE_VIDEO(exerciseId, trainingDayExerciseId))

                ExerciseVideoScreen(
                    exerciseId = exerciseId,
                    viewModel = viewModel,
                    onBack = { navController.navigate(TrainingProgramsRoutes.VIEW_EXERCISE_FROM_DAY(trainingDayExerciseId)) }
                )
            }
        }
    }
}