package com.example.gymtools.trainingprograms

import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.runtime.Composable
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.navigation.NavType
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import androidx.navigation.navArgument
import com.example.gymtools.settings.AddExerciseScreen
import com.example.gymtools.settings.EditExerciseScreen
import com.example.gymtools.settings.EditExercisesListScreen
import com.example.gymtools.settings.SettingsScreen
import com.example.gymtools.temp.AnimationScreen
import com.example.gymtools.temp.SoundPlayerScreen
import com.example.gymtools.temp.VideoPlayerScreen

@Composable
fun TrainingProgramsNavigation(
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val navController = rememberNavController()

    val startDestination = remember(viewModel.currentRoute) { viewModel.currentRoute }

    NavHost(
        navController = navController,
        startDestination = startDestination,
        modifier = modifier
            .fillMaxSize()
    ) {
        composable(route = "Programs list") {
            viewModel.updateCurrentRoute("Programs list")
            ProgramsListScreen(
                viewModel = viewModel,
                navController = navController
            )
        }
        composable(route = "Add program") {
            viewModel.updateCurrentRoute("Add program")
            AddProgramScreen(
                viewModel = viewModel,
                navController = navController
            )
        }
        composable(
            route = "Edit program/{programId}",
            arguments = listOf(navArgument("programId") { type = NavType.IntType })
        ) { backStackEntry ->
            val programId = backStackEntry.arguments?.getInt("programId")
            programId?.let {
                viewModel.updateCurrentRoute("Edit program/${programId}")
                EditProgramScreen(
                    programId = it,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = "Training days list/{programId}",
            arguments = listOf(navArgument("programId") { type = NavType.IntType })
        ) { backStackEntry ->
            val programId = backStackEntry.arguments?.getInt("programId")
            programId?.let {
                viewModel.updateCurrentRoute("Training days list/${programId}")
                TrainingDaysListScreen(
                    programId = it,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = "Add training day/{programId}",
            arguments = listOf(navArgument("programId") { type = NavType.IntType })
        ) { backStackEntry ->
            val programId = backStackEntry.arguments?.getInt("programId")
            programId?.let {
                viewModel.updateCurrentRoute("Add training day/${programId}")
                AddTrainingDayScreen(
                    programId = it,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = "Edit training day/{trainingDayId}",
            arguments = listOf(
                navArgument("trainingDayId") { type = NavType.IntType },
            )
        ) { backStackEntry ->
            val trainingDayId = backStackEntry.arguments?.getInt("trainingDayId")
            if (trainingDayId != null) {
                viewModel.updateCurrentRoute("Edit training day/$trainingDayId")
                EditTrainingDayScreen(
                    trainingDayId = trainingDayId,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = "View training day/{trainingDayId}",
            arguments = listOf(
                navArgument("trainingDayId") { type = NavType.IntType }
            )
        ) { backStackEntry ->
            val trainingDayId = backStackEntry.arguments?.getInt("trainingDayId")
            if (trainingDayId != null) {
                viewModel.updateCurrentRoute("View training day/$trainingDayId")
                ViewTrainingDayScreen(
                    trainingDayId = trainingDayId,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = "Add exercise to day/{trainingDayId}",
            arguments = listOf(
                navArgument("trainingDayId") { type = NavType.IntType },
            )
        ) { backStackEntry ->
            val trainingDayId = backStackEntry.arguments?.getInt("trainingDayId")

            if (trainingDayId != null) {
                viewModel.updateCurrentRoute("Add exercise to day/${trainingDayId}")
                AddExerciseToDayScreen(
                    trainingDayId = trainingDayId,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = "Edit exercise from day/{trainingDayExerciseId}",
            arguments = listOf(
                navArgument("trainingDayExerciseId") { type = NavType.IntType },
            )
        ) { backStackEntry ->
            val trainingDayExerciseId = backStackEntry.arguments?.getInt("trainingDayExerciseId")

            if (trainingDayExerciseId != null) {
                viewModel.updateCurrentRoute("Edit exercise from day/${trainingDayExerciseId}")
                EditExerciseFromDayScreen(
                    trainingDayExerciseId = trainingDayExerciseId,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(
            route = "View exercise from day/{trainingDayExerciseId}",
            arguments = listOf(
                navArgument("trainingDayExerciseId") { type = NavType.IntType },
            )
        ) { backStackEntry ->
            val trainingDayExerciseId = backStackEntry.arguments?.getInt("trainingDayExerciseId")

            if (trainingDayExerciseId != null) {
                viewModel.updateCurrentRoute("View exercise from day/${trainingDayExerciseId}")
                ViewExerciseFromDayScreen(
                    trainingDayExerciseId = trainingDayExerciseId,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(route = "Settings") {
            viewModel.updateCurrentRoute("Settings")
            SettingsScreen(
                navController = navController
            )
        }
        composable(route = "Edit exercises list") {
            viewModel.updateCurrentRoute("Edit exercises list")
            EditExercisesListScreen(
                navController = navController,
                viewModel = viewModel
            )
        }
        composable(route = "Add exercise") {
            viewModel.updateCurrentRoute("Add exercise")
            AddExerciseScreen(
                navController = navController,
                viewModel = viewModel
            )
        }
        composable(
            route = "Edit exercise/{exerciseId}",
            arguments = listOf(navArgument("exerciseId") { type = NavType.IntType })
        ) { backStackEntry ->
            val exerciseId = backStackEntry.arguments?.getInt("exerciseId")
            exerciseId?.let {
                viewModel.updateCurrentRoute("Edit exercise/${exerciseId}")
                EditExerciseScreen(
                    exerciseId = it,
                    viewModel = viewModel,
                    navController = navController
                )
            }
        }
        composable(route = "Sound player") {
            viewModel.updateCurrentRoute("Sound player")
            SoundPlayerScreen(
                navController = navController
            )
        }
        composable(route = "Video player") {
            viewModel.updateCurrentRoute("Video player")
            VideoPlayerScreen(
                navController = navController
            )
        }
        composable(route = "Animation") {
            viewModel.updateCurrentRoute("Animation")
            AnimationScreen(
                navController = navController
            )
        }
    }
}