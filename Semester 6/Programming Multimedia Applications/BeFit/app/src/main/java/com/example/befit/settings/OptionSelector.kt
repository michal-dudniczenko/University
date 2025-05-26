package com.example.befit.settings

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import com.example.befit.common.CustomStringOptionPicker
import com.example.befit.common.CustomText
import com.example.befit.constants.ROW_HEIGHT
import com.example.befit.constants.Themes
import com.example.befit.constants.adaptiveWidth

@Composable
fun OptionSelector(
    currentValue: String,
    label: String,
    optionsList: List<String>,
    onValueSelected: (String) -> Unit,
    modifier: Modifier = Modifier
) {
    Row(
        verticalAlignment = Alignment.CenterVertically,
        horizontalArrangement = Arrangement.SpaceBetween,
        modifier = modifier
            .fillMaxWidth()
            .height(ROW_HEIGHT.dp)
            .clip(RoundedCornerShape(adaptiveWidth(16).dp))
            .background(color = Themes.PRIMARY)
            .padding(horizontal = 16.dp)
    ) {
        CustomText(label, modifier = Modifier.weight(1f))
        CustomStringOptionPicker(
            optionsList = optionsList,
            onValueSelected = onValueSelected,
            selectedValue = currentValue,
            modifier = Modifier.weight(1f)
        )
    }
    Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
}