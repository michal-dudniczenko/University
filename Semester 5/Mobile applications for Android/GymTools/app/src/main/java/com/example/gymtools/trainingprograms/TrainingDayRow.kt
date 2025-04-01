package com.example.gymtools.trainingprograms

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.gymtools.common.CustomText
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.bigFontSize
import com.example.gymtools.common.darkBackground
import com.example.gymtools.common.editColor
import com.example.gymtools.database.TrainingDay

@Composable
fun TrainingDayRow(
    trainingDay: TrainingDay,
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val isEditMode by viewModel.isEditMode

    Row(
        horizontalArrangement = Arrangement.Center,
        modifier = modifier
            .fillMaxWidth()
            .background(
                color = if (isEditMode) editColor else darkBackground,
                shape = RoundedCornerShape(adaptiveWidth(16).dp)
            )
            .clickable {
                navController.navigate(
                if (isEditMode) "Edit training day/${trainingDay.id}"
                else "View training day/${trainingDay.id}"
                )
            }
            .padding(adaptiveWidth(16).dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .horizontalScroll(rememberScrollState())
        ) {
            CustomText(
                text = trainingDay.name,
                fontSize = bigFontSize,
            )
        }
    }
    Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
}