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
import com.example.befit.R
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomText
import com.example.befit.common.Heading
import com.example.befit.constants.Strings
import com.example.befit.constants.TrainingProgramsRoutes
import com.example.befit.constants.adaptiveHeight
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.bigFontSize
import com.example.befit.constants.bright
import com.example.befit.constants.editColor
import com.example.befit.constants.mediumGreen

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
            icon = R.drawable.settings,
            description = "App settings",
            onClick = { navController.navigate(TrainingProgramsRoutes.SETTINGS) },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        if (programs.isEmpty()) {
            CustomFloatingButton(
                icon = R.drawable.add,
                description = "Add button",
                onClick = { navController.navigate(TrainingProgramsRoutes.ADD_PROGRAM) },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
            )
        } else {
            CustomFloatingButton(
                icon = if (isEditMode) R.drawable.edit_white else R.drawable.edit,
                color = if (isEditMode) editColor else bright,
                description = "Edit button",
                onClick = { isEditMode = !isEditMode },
                modifier = Modifier
                    .align(Alignment.BottomEnd)
                    .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
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
                ) {
                    CustomText(
                        text = Strings.NOTHING_HERE_YET,
                        fontSize = bigFontSize,
                        modifier = Modifier
                            .align(Alignment.Center)
                            .offset(y = adaptiveHeight(-75).dp)
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
                                .background(color = mediumGreen)
                                .clickable {
                                    navController.navigate(TrainingProgramsRoutes.ADD_PROGRAM)
                                }
                        ) {
                            CustomText(
                                text = Strings.ADD_PROGRAM,
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