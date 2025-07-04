package com.example.befit.common

import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.HorizontalDivider
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
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.mediumFontSize
import com.example.befit.constants.smallFontSize
import com.example.befit.database.Exercise
import com.example.befit.health.caloriecalculator.ActivityLevel
import com.example.befit.health.caloriecalculator.ActivityLevels

@Composable
fun CustomIntPicker(
    label: String,
    onValueChange: (Int) -> Unit,
    modifier: Modifier = Modifier,
    selectedValue: Int? = null,
    imageId: Int = Themes.NUMBERS_ON_PRIMARY
) {
    var textValue by remember { mutableStateOf(
        if (selectedValue == 0 || selectedValue == null) "" else selectedValue.toString()
    ) }

    OutlinedTextField(
        textStyle = TextStyle(
            color = Themes.ON_PRIMARY,
            fontSize = adaptiveWidth(mediumFontSize).sp,
            fontWeight = FontWeight.Bold
        ),
        colors = OutlinedTextFieldDefaults.colors(
            focusedBorderColor = Themes.ON_BACKGROUND,
            unfocusedBorderColor = Themes.ON_BACKGROUND,
            focusedTextColor = Themes.ON_PRIMARY,
            unfocusedTextColor = Themes.ON_PRIMARY,
            focusedLabelColor = Themes.ON_BACKGROUND,
            unfocusedLabelColor = Themes.ON_PRIMARY,
            focusedTrailingIconColor = Themes.ON_PRIMARY,
            unfocusedTrailingIconColor = Themes.ON_PRIMARY,
            focusedContainerColor = Themes.PRIMARY,
            unfocusedContainerColor = Themes.PRIMARY,
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
                modifier = Modifier.height(24.dp)
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
    label: String,
    onValueChange: (Float) -> Unit,
    modifier: Modifier = Modifier,
    selectedValue: Float? = null,
    imageId: Int = Themes.NUMBERS_ON_PRIMARY,
) {
    var textValue by remember { mutableStateOf(
        if (selectedValue == 0f || selectedValue == null) "" else floatToString(selectedValue)
    ) }

    OutlinedTextField(
        textStyle = TextStyle(
            color = Themes.ON_PRIMARY,
            fontSize = adaptiveWidth(mediumFontSize).sp,
            fontWeight = FontWeight.Bold
        ),
        colors = OutlinedTextFieldDefaults.colors(
            focusedBorderColor = Themes.ON_BACKGROUND,
            unfocusedBorderColor = Themes.ON_BACKGROUND,
            focusedTextColor = Themes.ON_PRIMARY,
            unfocusedTextColor = Themes.ON_PRIMARY,
            focusedLabelColor = Themes.ON_BACKGROUND,
            unfocusedLabelColor = Themes.ON_PRIMARY,
            focusedTrailingIconColor = Themes.ON_PRIMARY,
            unfocusedTrailingIconColor = Themes.ON_PRIMARY,
            focusedContainerColor = Themes.PRIMARY,
            unfocusedContainerColor = Themes.PRIMARY,
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
                modifier = Modifier.height(24.dp)
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
    onValueChange: (String) -> Unit,
    label: String,
    modifier: Modifier = Modifier,
    selectedValue: String? = null,
    imageId: Int = Themes.ABC_ON_PRIMARY,
) {
    var textValue by remember { mutableStateOf(selectedValue ?: "") }

    OutlinedTextField(
        textStyle = TextStyle(
            color = Themes.ON_PRIMARY,
            fontSize = adaptiveWidth(mediumFontSize).sp,
            fontWeight = FontWeight.Bold
        ),
        colors = OutlinedTextFieldDefaults.colors(
            focusedBorderColor = Themes.ON_BACKGROUND,
            unfocusedBorderColor = Themes.ON_BACKGROUND,
            focusedTextColor = Themes.ON_PRIMARY,
            unfocusedTextColor = Themes.ON_PRIMARY,
            focusedLabelColor = Themes.ON_BACKGROUND,
            unfocusedLabelColor = Themes.ON_PRIMARY,
            focusedTrailingIconColor = Themes.ON_PRIMARY,
            unfocusedTrailingIconColor = Themes.ON_PRIMARY,
            focusedContainerColor = Themes.PRIMARY,
            unfocusedContainerColor = Themes.PRIMARY,
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
                modifier = Modifier.height(24.dp)
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
    exercises: List<Exercise>,
    onExerciseSelected: (Exercise) -> Unit,
    modifier: Modifier = Modifier,
    selectedExercise: Exercise? = null,
) {
    var expanded by remember { mutableStateOf(false) }
    var selectedText by remember { mutableStateOf(selectedExercise?.name ?: Strings.SELECT_EXERCISE) }

    Box(modifier = modifier.fillMaxWidth()) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .background(
                    color = Themes.PRIMARY,
                    shape = RoundedCornerShape(4.dp)
                )
                .border(
                    width = 1.dp,
                    color = Themes.ON_BACKGROUND,
                    shape = RoundedCornerShape(4.dp)
                )
                .clickable {
                    expanded = !expanded
                }
                .padding(16.dp)
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
            exercises.forEachIndexed { index, exercise ->
                DropdownMenuItem(
                    text = { Text(text = exercise.name) },
                    onClick = {
                        onExerciseSelected(exercise)
                        selectedText = exercise.name
                        expanded = false
                    }
                )
                if (index < exercises.lastIndex) {
                    HorizontalDivider(
                        thickness = 2.dp
                    )
                }
            }
        }
    }
}

@Composable
fun CustomSexPicker(
    onValueSelected: (Boolean) -> Unit,
    modifier: Modifier = Modifier,
    selectedValue: String? = null
) {
    var expanded by remember { mutableStateOf(false) }
    var selectedText by remember { mutableStateOf(selectedValue ?: Strings.SELECT_SEX) }

    val options = listOf(Strings.MALE, Strings.FEMALE)

    Box(modifier = modifier.fillMaxWidth()) {
        Row(
            horizontalArrangement = Arrangement.Center,
            modifier = Modifier
                .fillMaxWidth()
                .background(
                    color = Themes.PRIMARY,
                    shape = RoundedCornerShape(4.dp)
                )
                .border(
                    width = 1.dp,
                    color = Themes.ON_BACKGROUND,
                    shape = RoundedCornerShape(4.dp)
                )
                .clickable {
                    expanded = !expanded
                }
                .padding(16.dp)
        ) {
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .horizontalScroll(rememberScrollState())
            ) {
                CustomText(
                    text = selectedText,
                    isBoldFont = false,
                    fontSize = smallFontSize
                )
            }

        }

        DropdownMenu(
            expanded = expanded,
            onDismissRequest = { expanded = false },
            modifier = Modifier.fillMaxWidth(0.8f * 0.7f)
        ) {
            options.forEachIndexed { index, option ->
                DropdownMenuItem(
                    text = { Text(text = option) },
                    onClick = {
                        onValueSelected(
                            when (option) {
                                options[0] -> true
                                else -> false
                            }
                        )
                        selectedText = option
                        expanded = false
                    }
                )
                if (index < options.lastIndex) {
                    HorizontalDivider(
                        thickness = 2.dp
                    )
                }
            }
        }
    }
}

@Composable
fun CustomActivityLevelPicker(
    onValueSelected: (Int) -> Unit,
    modifier: Modifier = Modifier,
    selectedValue: ActivityLevel? = null
) {
    var expanded by remember { mutableStateOf(false) }
    var selectedText by remember { mutableStateOf(selectedValue?.name ?: Strings.ACTIVITY_LEVEL) }

    val activityLevels = ActivityLevels.getLevels()

    Box(modifier = modifier.fillMaxWidth()) {
        Row(
            horizontalArrangement = Arrangement.Center,
            modifier = Modifier
                .fillMaxWidth()
                .background(
                    color = Themes.PRIMARY,
                    shape = RoundedCornerShape(4.dp)
                )
                .border(
                    width = 1.dp,
                    color = Themes.ON_BACKGROUND,
                    shape = RoundedCornerShape(4.dp)
                )
                .clickable {
                    expanded = !expanded
                }
                .padding(16.dp)
        ) {
            Row(
                modifier = Modifier
                    .fillMaxWidth()
                    .horizontalScroll(rememberScrollState())
            ) {
                CustomText(
                    text = selectedText,
                    isBoldFont = false,
                    fontSize = smallFontSize
                )
            }

        }

        DropdownMenu(
            expanded = expanded,
            onDismissRequest = { expanded = false },
            modifier = Modifier.fillMaxWidth(0.8f * 0.7f)
        ) {
            activityLevels.forEachIndexed { index, option ->
                DropdownMenuItem(
                    text = { Text(text = option.name) },
                    onClick = {
                        onValueSelected(option.level)
                        selectedText = option.name
                        expanded = false
                    }
                )
                if (index < activityLevels.lastIndex) {
                    HorizontalDivider(
                        thickness = 2.dp
                    )
                }
            }
        }

    }
}


@Composable
fun CustomStringOptionPicker(
    optionsList: List<String>,
    onValueSelected: (String) -> Unit,
    modifier: Modifier = Modifier,
    selectedValue: String
) {
    var expanded by remember { mutableStateOf(false) }
    var selectedText by remember { mutableStateOf(selectedValue) }

    Box(
        modifier = modifier
            .fillMaxWidth()
    ) {
        Row(
            horizontalArrangement = Arrangement.Center,
            modifier = Modifier
                .fillMaxWidth()
                .background(
                    color = Themes.PRIMARY,
                    shape = RoundedCornerShape(4.dp)
                )
                .border(
                    width = 1.dp,
                    color = Themes.ON_BACKGROUND,
                    shape = RoundedCornerShape(4.dp)
                )
                .clickable {
                    expanded = !expanded
                }
                .padding(8.dp)
        ) {
            Row(
                modifier = Modifier
                    .horizontalScroll(rememberScrollState())
            ) {
                CustomText(
                    text = selectedText,
                    isBoldFont = false,
                    fontSize = smallFontSize
                )
            }

        }

        DropdownMenu(
            expanded = expanded,
            onDismissRequest = { expanded = false },
            modifier = Modifier.fillMaxWidth(0.8f * 0.7f)
        ) {
            optionsList.forEachIndexed { index, option ->
                DropdownMenuItem(
                    text = { Text(text = option) },
                    onClick = {
                        onValueSelected(option)
                        selectedText = option
                        expanded = false
                    }
                )
                if (index < optionsList.lastIndex) {
                    HorizontalDivider(
                        thickness = 2.dp
                    )
                }
            }
        }

    }
}