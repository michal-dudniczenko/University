package com.example.befit.health.weighthistory

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomText
import com.example.befit.common.Heading
import com.example.befit.constants.HealthRoutes
import com.example.befit.constants.PADDING_BOTTOM
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes
import com.example.befit.constants.WeightHistoryRoutes
import com.example.befit.constants.adaptiveWidth
import com.example.befit.constants.bigFontSize

@Composable
fun WeightHistoryScreen(
    navController: NavHostController,
    topLevelNavController: NavHostController,
    viewModel: WeightHistoryViewModel,
    modifier: Modifier = Modifier
) {
    val weights by viewModel.weights

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.BACK_ON_SECONDARY,
            description = "Back button",
            onClick = { topLevelNavController.navigate(HealthRoutes.TOOLS_LIST) },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = 30.dp, y = (-30).dp)
        )
        CustomFloatingButton(
            icon = Themes.ADD_ON_SECONDARY,
            description = "Add button",
            onClick = { navController.navigate(WeightHistoryRoutes.ADD) },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .offset(x = (-30).dp, y = (-30).dp)
        )
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(Strings.WEIGHT_HISTORY)
            if (weights.isEmpty()) {
                Box(
                    modifier = Modifier
                        .fillMaxSize()
                        .padding(bottom = PADDING_BOTTOM.dp)
                ) {
                    CustomText(
                        text = Strings.NOTHING_HERE_YET,
                        color = Themes.ON_BACKGROUND,
                        fontSize = bigFontSize,
                        modifier = Modifier
                            .align(Alignment.Center)
                    )
                }
            } else {
                LazyColumn(
                    horizontalAlignment = Alignment.CenterHorizontally,
                    modifier = modifier
                        .fillMaxWidth(0.95f)
                        .fillMaxHeight()
                ) {
                    items(weights) { weight ->
                        WeightRow(
                            weight = weight,
                            onClick = { navController.navigate(WeightHistoryRoutes.EDIT(weight.id)) }
                        )
                        Spacer(modifier = Modifier.height(adaptiveWidth(22).dp))
                    }
                    item { Spacer(modifier = Modifier.height(adaptiveWidth(100).dp))}
                }
            }
        }
    }
}