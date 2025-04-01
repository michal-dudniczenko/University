package com.example.gymtools.trainingprograms

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.OutlinedTextFieldDefaults
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.navigation.NavHostController
import com.example.gymtools.R
import com.example.gymtools.common.CustomFloatingButton
import com.example.gymtools.common.CustomText
import com.example.gymtools.common.Heading
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.bigFontSize
import com.example.gymtools.common.bright
import com.example.gymtools.common.darkBackground
import com.example.gymtools.common.mediumFontSize
import kotlinx.coroutines.launch

@Composable
fun ViewExerciseFromDayScreen(
    trainingDayExerciseId: Int,
    navController: NavHostController,
    viewModel: TrainingProgramsViewModel,
    modifier: Modifier = Modifier
) {
    val trainingDayExercise = viewModel.getTrainingDayExerciseById(trainingDayExerciseId)

    val exercise = viewModel.getExerciseById(trainingDayExercise?.exerciseId ?: 0)

    val trainingDayId = trainingDayExercise?.trainingDayId ?: 0

    var viewModelNotes by viewModel.currentNotes

    var notes by remember { mutableStateOf(viewModelNotes.ifEmpty { exercise?.notes ?: "" }) }

    val restTime = trainingDayExercise?.restTime ?: ""

    val coroutineScope = rememberCoroutineScope()

    Box(
        modifier = modifier.fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.back,
            description = "Back button",
            onClick = {
                coroutineScope.launch {
                    viewModel.updateExercise(
                        id = exercise?.id ?: 0,
                        name = exercise?.name ?: "",
                        notes = notes
                    )
                    viewModelNotes = ""
                    navController.navigate("View training day/$trainingDayId")
                }
            },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading("Exercise info")
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight(0.85f)
                    .background(
                        shape = RoundedCornerShape(adaptiveWidth(16).dp),
                        color = darkBackground
                    )
            ) {
                Spacer(Modifier.height(adaptiveWidth(16).dp))
                Row(
                    horizontalArrangement = Arrangement.Center,
                    modifier = Modifier
                        .fillMaxWidth(0.9f)
                        .background(
                            color = bright,
                            shape = RoundedCornerShape(adaptiveWidth(10).dp)
                        )
                        .padding(8.dp)
                ) {
                    CustomText(
                        text = exercise?.name ?: "",
                        color = darkBackground,
                        fontSize = bigFontSize,
                    )
                }
                Spacer(Modifier.height(adaptiveWidth(32).dp))
                Row(
                    modifier = Modifier
                        .fillMaxWidth(0.85f)
                ) {
                    CustomText(
                        text = "Rest time:  $restTime"
                    )
                }
                Spacer(Modifier.height(adaptiveWidth(16).dp))
                OutlinedTextField(
                    textStyle = TextStyle(
                        color = bright,
                        fontSize = adaptiveWidth(mediumFontSize).sp,
                        fontWeight = FontWeight.Bold
                    ),
                    colors = OutlinedTextFieldDefaults.colors(
                        focusedBorderColor = bright,
                        unfocusedBorderColor = bright,
                        focusedTextColor = bright,
                        unfocusedTextColor = bright,
                        focusedLabelColor = bright,
                        unfocusedLabelColor = bright,
                        focusedContainerColor = darkBackground,
                        unfocusedContainerColor = darkBackground,
                    ),
                    value = notes,
                    onValueChange = { input: String ->
                        notes = input
                        viewModelNotes = input
                    },
                    label = { Text("Notes") },
                    modifier = Modifier
                        .fillMaxWidth(0.9f)
                        .fillMaxHeight(0.95f)
                )
            }
        }
    }
}