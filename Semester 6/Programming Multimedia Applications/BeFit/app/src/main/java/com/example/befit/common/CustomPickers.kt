package com.example.befit.common

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.OutlinedTextFieldDefaults
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.example.befit.R
import com.example.befit.database.Exercise

@Composable
fun CustomIntPicker(
    selectedValue: Int? = null,
    label: String,
    imageId: Int,
    onValueChange: (Int) -> Unit,
    modifier: Modifier = Modifier
) {
    var textValue by remember { mutableStateOf(
        if (selectedValue == 0 || selectedValue == null) "" else selectedValue.toString()
    ) }

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
            focusedTrailingIconColor = bright,
            unfocusedTrailingIconColor = bright,
            focusedContainerColor = darkBackground,
            unfocusedContainerColor = darkBackground,
        ),
        singleLine = true,
        value = textValue,
        onValueChange = { input: String ->
            if (input.isEmpty() || input.toIntOrNull() != null) {
                textValue = input
                if (input.isNotEmpty()) {
                    onValueChange(input.toInt())
                } else {
                    onValueChange(0)
                }
            }
        },
        label = { Text(label) },
        trailingIcon = {
            Image(
                painter = painterResource(id = imageId),
                contentDescription = label,
                modifier = Modifier.height(adaptiveWidth(24).dp)
            )
        },
        keyboardOptions = KeyboardOptions.Default.copy(
            keyboardType = KeyboardType.Number
        ),
        modifier = modifier.fillMaxWidth()
    )
}

@Composable
fun CustomFloatPicker(
    selectedValue: Float? = null,
    label: String,
    imageId: Int,
    onValueChange: (Float) -> Unit,
    modifier: Modifier = Modifier
) {
    var textValue by remember { mutableStateOf(
        if (selectedValue == 0f || selectedValue == null) "" else floatToString(selectedValue)
    ) }

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
            focusedTrailingIconColor = bright,
            unfocusedTrailingIconColor = bright,
            focusedContainerColor = darkBackground,
            unfocusedContainerColor = darkBackground,
        ),
        singleLine = true,
        value = textValue,
        onValueChange = { input: String ->
            if (input.isEmpty() || input.toFloatOrNull() != null) {
                textValue = input
                if (input.isNotEmpty()) {
                    onValueChange(input.toFloat())
                } else {
                    onValueChange(0f)
                }
            }
        },
        label = { Text(label) },
        trailingIcon = {
            Image(
                painter = painterResource(id = imageId),
                contentDescription = label,
                modifier = Modifier.height(adaptiveWidth(24).dp)
            )
        },
        keyboardOptions = KeyboardOptions.Default.copy(
            keyboardType = KeyboardType.Number
        ),
        modifier = modifier.fillMaxWidth()
    )
}

@Composable
fun CustomStringPicker(
    selectedValue: String? = null,
    onValueChange: (String) -> Unit,
    label: String,
    imageId: Int = R.drawable.abc,
    modifier: Modifier = Modifier
) {
    var textValue by remember { mutableStateOf(selectedValue ?: "") }

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
            focusedTrailingIconColor = bright,
            unfocusedTrailingIconColor = bright,
            focusedContainerColor = darkBackground,
            unfocusedContainerColor = darkBackground,
        ),
        singleLine = true,
        value = textValue,
        onValueChange = { input: String ->
            textValue = input
            onValueChange(input)
        },
        label = { Text(label) },
        trailingIcon = {
            Image(
                painter = painterResource(id = imageId),
                contentDescription = "Select name",
                modifier = Modifier.height(adaptiveWidth(24).dp)
            )
        },
        keyboardOptions = KeyboardOptions.Default.copy(
            keyboardType = KeyboardType.Text
        ),
        modifier = modifier.fillMaxWidth()
    )
}

@Composable
fun CustomExercisePicker(
    selectedExercise: Exercise? = null,
    exercises: List<Exercise>,
    onExerciseSelected: (Exercise) -> Unit,
    modifier: Modifier = Modifier
) {
    var expanded by remember { mutableStateOf(false) }
    var selectedText by remember { mutableStateOf(selectedExercise?.name ?: "Select exercise") }

    Box(modifier = modifier.fillMaxWidth()) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .background(
                    color = darkBackground,
                    shape = RoundedCornerShape(4.dp)
                )
                .border(
                    width = 1.dp,
                    color = bright,
                    shape = RoundedCornerShape(4.dp)
                )
                .clickable {
                    expanded = !expanded
                }
                .padding(adaptiveWidth(16).dp)
        ) {
            CustomText(
                text = selectedText
            )
        }

        DropdownMenu(
            expanded = expanded,
            onDismissRequest = { expanded = false },
            modifier = Modifier.fillMaxWidth(0.8f * 0.95f)
        ) {
            exercises.forEach { exercise ->
                DropdownMenuItem(
                    text = { Text(text = exercise.name) },
                    onClick = {
                        onExerciseSelected(exercise)
                        selectedText = exercise.name
                        expanded = false
                    }
                )
            }
        }
    }
}