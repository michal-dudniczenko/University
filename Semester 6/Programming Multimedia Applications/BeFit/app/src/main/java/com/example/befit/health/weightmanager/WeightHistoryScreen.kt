package com.example.befit.health.weightmanager

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.runtime.Composable
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.befit.R
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.CustomText
import com.example.befit.common.HEALTH_SCREENS
import com.example.befit.common.Heading
import com.example.befit.common.adaptiveHeight
import com.example.befit.common.adaptiveWidth
import com.example.befit.common.bigFontSize

@Composable
fun WeightHistoryScreen(
    navController: NavHostController,
    topLevelNavController: NavHostController,
    viewModel: WeightManagerViewModel,
    modifier: Modifier = Modifier
) {
    val weights by viewModel.weights.collectAsState()

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.back,
            description = "Back button",
            onClick = { topLevelNavController.navigate(HEALTH_SCREENS[0]) },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        CustomFloatingButton(
            icon = R.drawable.add,
            description = "Add button",
            onClick = { navController.navigate("Add weight") },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
        )
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading("Weight history")
            if (weights.isEmpty()) {
                Box(
                    modifier = Modifier
                        .fillMaxSize()
                ) {
                    CustomText(
                        text = "Nothing here yet!",
                        fontSize = bigFontSize,
                        modifier = Modifier
                            .align(Alignment.Center)
                            .offset(y = adaptiveHeight(-75).dp)
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
                        WeightRow(weight, navController)
                        Spacer(modifier = Modifier.height(adaptiveWidth(22).dp))
                    }
                    item { Spacer(modifier = Modifier.height(adaptiveWidth(100).dp))}
                }
            }
        }
    }
}