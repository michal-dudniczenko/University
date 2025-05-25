package com.example.befit.health.weighthistory

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomText
import com.example.befit.common.WeightHistoryRoutes
import com.example.befit.common.adaptiveWidth
import com.example.befit.common.darkBackground
import com.example.befit.common.formatDateFromLong
import com.example.befit.database.Weight
import java.util.Locale

@Composable
fun WeightRow(
    weight: Weight,
    onClick: () -> Unit,
    modifier: Modifier = Modifier
) {
    Row(
        horizontalArrangement = Arrangement.SpaceBetween,
        modifier = modifier
            .fillMaxWidth()
            .clip(RoundedCornerShape(adaptiveWidth(16).dp))
            .background(color = darkBackground)
            .clickable(onClick = onClick)
            .padding(adaptiveWidth(16).dp)
    ) {
        CustomText(
            text = formatDateFromLong(weight.date),
        )
        CustomText(
            text = "${String.format(Locale.US, "%.1f", weight.weight)} kg",
        )
    }
}