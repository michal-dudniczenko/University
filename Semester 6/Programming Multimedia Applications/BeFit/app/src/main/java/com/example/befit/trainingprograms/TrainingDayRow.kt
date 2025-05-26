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
import com.example.befit.constants.ROW_HEIGHT
import com.example.befit.constants.Themes
import com.example.befit.constants.TrainingProgramsRoutes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.mediumFontSize

@Composable
fun TrainingDayRow(
    trainingDayId: Int,
    trainingDayName: String,
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
                .background(color = if (isEditMode) Themes.EDIT_COLOR else Themes.PRIMARY)
                .clickable {
                    if (isEditMode) {
                        navController.navigate(TrainingProgramsRoutes.EDIT_TRAINING_DAY(trainingDayId))
                    } else {
                        navController.navigate(TrainingProgramsRoutes.VIEW_TRAINING_DAY(trainingDayId))
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
                    text = trainingDayName,
                    color = if (isEditMode) Themes.ON_EDIT else Themes.ON_PRIMARY,
                    fontSize = mediumFontSize,
                )
            }
        }
        if (isEditMode) {
            DeleteButton(
                onClick = { viewModel.deleteTrainingDay(trainingDayId) }
            )
        }
    }
    Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
}