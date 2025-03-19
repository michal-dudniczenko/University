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
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.gymtools.common.CustomText
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.bigFontSize
import com.example.gymtools.common.darkBackground
import com.example.gymtools.database.Program

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
            .background(
                color = darkBackground,
                shape = RoundedCornerShape(adaptiveWidth(16).dp)
            )
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
                fontSize = bigFontSize,
            )
        }
    }
    Spacer(modifier = Modifier.height(adaptiveWidth(32).dp))
}