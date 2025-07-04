package com.example.befit.trainingprograms

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomStringPicker
import com.example.befit.common.Heading
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.TrainingProgramsRoutes

@Composable
fun EditProgramScreen(
    programId: Int,
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val programs by viewModel.programs

    val program = programs.find { it.id == programId }

    var selectedName by remember { mutableStateOf(program?.name ?: "") }

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.BACK_ON_SECONDARY,
            description = "Back button",
            onClick = {
                if (selectedName.isNotEmpty()) {
                    viewModel.updateProgram(id = programId, name = selectedName)
                }
                navController.navigate(TrainingProgramsRoutes.PROGRAMS_LIST)
            },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = 30.dp, y = (-30).dp)
        )
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
            Heading(Strings.EDIT_PROGRAM)
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                verticalArrangement = Arrangement.Center,
                modifier = modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
            ) {
                CustomStringPicker(
                    selectedValue = selectedName,
                    label = Strings.PROGRAM_NAME,
                    onValueChange = { selectedName = it }
                )
                Spacer(modifier = Modifier.height(200.dp))
            }
        }
    }
}