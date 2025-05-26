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
import com.example.befit.constants.PADDING_BOTTOM
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.TrainingProgramsRoutes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.bigFontSize

@Composable
fun ProgramsListScreen(
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val programs by viewModel.programs
    var isEditMode by viewModel.isEditMode

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.SETTINGS_ON_SECONDARY,
            description = "App settings",
            onClick = { navController.navigate(TrainingProgramsRoutes.SETTINGS) },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = 30.dp, y = (-30).dp)
        )
        if (programs.isEmpty()) {
            CustomFloatingButton(
                icon = Themes.ADD_ON_SECONDARY,
                description = "Add button",
                onClick = { navController.navigate(TrainingProgramsRoutes.ADD_PROGRAM) },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = (-30).dp, y = (-30).dp)
            )
        } else {
            CustomFloatingButton(
                icon = if (isEditMode) Themes.EDIT_ON_EDIT else Themes.EDIT_ON_SECONDARY,
                color = if (isEditMode) Themes.EDIT_COLOR else Themes.SECONDARY,
                description = "Edit button",
                onClick = { isEditMode = !isEditMode },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = (-30).dp, y = (-30).dp)
            )
        }
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(Strings.YOUR_PROGRAMS)
            if (programs.isEmpty()) {
                Box(
                    modifier = Modifier
                        .fillMaxSize()
                        .padding(bottom = PADDING_BOTTOM.dp)
                ) {
                    CustomText(
                        text = Strings.NOTHING_HERE_YET,
                        color = Themes.ON_BACKGROUND,
                        fontSize = bigFontSize,
                        modifier = Modifier
                            .align(Alignment.Center)
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
                                    navController.navigate(TrainingProgramsRoutes.ADD_PROGRAM)
                                }
                        ) {
                            CustomText(
                                text = Strings.ADD_PROGRAM,
                                color = Themes.ON_ADD_CONFIRM,
                                modifier = Modifier.padding(adaptiveWidth(16).dp)
                            )
                        }
                        Spacer(modifier = Modifier.height(adaptiveWidth(28).dp))
                    }
                    for (program in programs) {
                        ProgramRow(
                            programId = program.id,
                            programName = program.name,
                            viewModel = viewModel,
                            navController = navController
                        )
                    }
                }
            }
        }
    }
}