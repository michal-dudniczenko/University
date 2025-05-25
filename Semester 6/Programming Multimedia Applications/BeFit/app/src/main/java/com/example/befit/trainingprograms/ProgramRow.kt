package com.example.befit.trainingprograms

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomText
import com.example.befit.common.DeleteButton
import com.example.befit.common.ROW_HEIGHT
import com.example.befit.common.TrainingProgramsRoutes
import com.example.befit.common.adaptiveWidth
import com.example.befit.common.darkBackground
import com.example.befit.common.editColor
import com.example.befit.common.mediumFontSize

@Composable
fun ProgramRow(
    programId: Int,
    programName: String,
    viewModel: TrainingProgramsViewModel,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    var isEditMode by viewModel.isEditMode

    Row(
        horizontalArrangement = Arrangement.SpaceBetween,
        modifier = modifier
            .height(adaptiveWidth(ROW_HEIGHT).dp)
            .fillMaxWidth()
    ) {
        // name row
        Row(
            verticalAlignment = Alignment.CenterVertically,
            horizontalArrangement = Arrangement.Center,
            modifier = Modifier
                .weight(1f)
                .fillMaxHeight()
                .clip(RoundedCornerShape(adaptiveWidth(16).dp))
                .background(color = if (isEditMode) editColor else darkBackground)
                .clickable {
                    if (isEditMode) {
                        navController.navigate(TrainingProgramsRoutes.EDIT_PROGRAM(programId))
                    } else {
                        navController.navigate(TrainingProgramsRoutes.TRAINING_DAYS_LIST(programId))
                    }
                }
                .padding(adaptiveWidth(16).dp)
        ) {
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .horizontalScroll(rememberScrollState())
            ) {
                CustomText(
                    text = programName,
                    fontSize = mediumFontSize,
                )
            }
        }
        if (isEditMode) {
            DeleteButton(
                onClick = { viewModel.deleteProgram(programId) }
            )
        }
    }
    Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
}

