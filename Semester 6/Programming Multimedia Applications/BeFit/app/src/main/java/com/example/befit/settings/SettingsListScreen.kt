package com.example.befit.settings

import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import com.example.befit.common.Heading
import com.example.befit.constants.AppThemes
import com.example.befit.constants.Languages
import com.example.befit.constants.Strings
import com.example.befit.constants.Themes

@Composable
fun SettingsListScreen(
    viewModel: SettingsViewModel,
    modifier: Modifier = Modifier
) {
    val appSettings by viewModel.appSettings

    Box (
        modifier = modifier
            .fillMaxSize()
    ) {
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(Strings.APP_SETTINGS)
            if (appSettings == null) {
                Box(
                    modifier = Modifier.fillMaxSize()
                ) {
                    CircularProgressIndicator(
                        color = Themes.ON_BACKGROUND,
                        modifier = Modifier.align(Alignment.Center)
                    )
                    return
                }
            }
            Column(
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = Modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight()
                    .verticalScroll(rememberScrollState())
            ) {
                OptionSelector(
                    currentValue = appSettings!!.language,
                    label = Strings.LANGUAGE,
                    optionsList = Languages.values,
                    onValueSelected = { viewModel.updateAppSettings(language = it) }
                )
                OptionSelector(
                    currentValue = appSettings!!.theme,
                    label = Strings.THEME,
                    optionsList = AppThemes.values,
                    onValueSelected = { viewModel.updateAppSettings(theme = it) }
                )
            }
        }
    }
}