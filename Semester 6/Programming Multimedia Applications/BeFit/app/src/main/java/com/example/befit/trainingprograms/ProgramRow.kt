package com.example.befit.trainingprograms

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
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomText
import com.example.befit.common.adaptiveWidth
import com.example.befit.common.bigFontSize
import com.example.befit.common.darkBackground
import com.example.befit.common.mediumFontSize
import com.example.befit.database.Program

@Composable
fun ProgramRow(
    program: Program,
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    Row(
        horizontalArrangement = Arrangement.Center,
        modifier = modifier
            .fillMaxWidth()
            .clip(RoundedCornerShape(adaptiveWidth(16).dp))
            .background(color = darkBackground)
            .clickable { navController.navigate("Training days list/${program.id}") }
            .padding(adaptiveWidth(16).dp)
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .horizontalScroll(rememberScrollState())
        ) {
            CustomText(
                text = program.name,
                fontSize = mediumFontSize,
            )
        }
    }
    Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
}