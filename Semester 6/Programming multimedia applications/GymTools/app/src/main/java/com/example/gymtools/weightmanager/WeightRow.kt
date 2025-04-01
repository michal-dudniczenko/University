package com.example.gymtools.weightmanager

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.gymtools.common.CustomText
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.darkBackground
import com.example.gymtools.common.formatDateFromLong
import com.example.gymtools.database.Weight
import java.util.Locale

@Composable
fun WeightRow(
    weight: Weight,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    Row(
        horizontalArrangement = Arrangement.SpaceBetween,
        modifier = modifier
            .fillMaxWidth()
            .background(
                color = darkBackground,
                shape = RoundedCornerShape(adaptiveWidth(16).dp)
            )
            .clickable { navController.navigate("Edit weight/${weight.id}") }
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