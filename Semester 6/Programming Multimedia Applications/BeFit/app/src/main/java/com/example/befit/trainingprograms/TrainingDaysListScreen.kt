package com.example.befit.trainingprograms

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomText
import com.example.befit.common.Heading
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.TrainingProgramsRoutes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.bigFontSize

@Composable
fun TrainingDaysListScreen(
    programId: Int,
    viewModel: TrainingProgramsViewModel,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    var isEditMode by viewModel.isEditMode

    val programs by viewModel.programs
    val trainingDays by viewModel.trainingDays

    val program = programs.find { it.id == programId }
    val trainingDaysFromProgram = trainingDays.filter { it.programId == programId }

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.BACK_ON_SECONDARY,
            description = "Back button",
            onClick = {
                navController.navigate(TrainingProgramsRoutes.PROGRAMS_LIST)
                isEditMode = false
            },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        if (trainingDaysFromProgram.isEmpty()) {
            CustomFloatingButton(
                icon = Themes.ADD_ON_SECONDARY,
                description = "Add button",
                onClick = { navController.navigate(TrainingProgramsRoutes.ADD_TRAINING_DAY(programId)) },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
            )
        } else {
            CustomFloatingButton(
                icon = if (isEditMode) Themes.EDIT_ON_EDIT else Themes.EDIT_ON_SECONDARY,
                color = if (isEditMode) Themes.EDIT_COLOR else Themes.SECONDARY,
                description = "Edit button",
                onClick = { isEditMode = !isEditMode },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
            )
        }
        if (program == null) {
            Box(
                modifier = Modifier.fillMaxSize()
            ) {
                CircularProgressIndicator(
                    color = Themes.ON_BACKGROUND,
                    modifier = Modifier.align(Alignment.Center)
                )
                return
            }
        }
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(program?.name ?: "")
            if (trainingDaysFromProgram.isEmpty()) {
                Box(
                    modifier = Modifier
                        .fillMaxSize()
                ) {
                    CustomText(
                        text = Strings.NOTHING_HERE_YET,
                        color = Themes.ON_BACKGROUND,
                        fontSize = bigFontSize,
                        modifier = Modifier
                            .align(Alignment.Center)
                            .offset(y = adaptiveWidth(-75).dp)
                    )
                }
            } else {
                Column(
                    horizontalAlignment = Alignment.CenterHorizontally,
                    modifier = Modifier
                        .fillMaxWidth(0.95f)
                        .fillMaxHeight()
                        .verticalScroll(rememberScrollState())
                ) {
                    if (isEditMode) {
                        Box(
                            contentAlignment = Alignment.Center,
                            modifier = modifier
                                .fillMaxWidth()
                                .clip(RoundedCornerShape(adaptiveWidth(16).dp))
                                .background(color = Themes.ADD_CONFIRM_COLOR)
                                .clickable {
                                    navController.navigate(TrainingProgramsRoutes.ADD_TRAINING_DAY(programId))
                                }
                        ) {
                            CustomText(
                                text = Strings.ADD_TRAINING_DAY,
                                color = Themes.ON_ADD_CONFIRM,
                                modifier = Modifier.padding(adaptiveWidth(16).dp)
                            )
                        }
                        Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
                    }
                    for (trainingDay in trainingDaysFromProgram) {
                        TrainingDayRow(
                            trainingDayId = trainingDay.id,
                            trainingDayName = trainingDay.name,
                            viewModel = viewModel,
                            navController= navController
                        )
                    }
                }
            }
        }
    }
}