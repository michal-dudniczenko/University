package com.example.gymtools.temp

import android.media.MediaPlayer
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.runtime.Composable
import androidx.compose.runtime.DisposableEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.gymtools.R
import com.example.gymtools.common.CustomFloatingButton
import com.example.gymtools.common.CustomText
import com.example.gymtools.common.Heading
import com.example.gymtools.common.adaptiveWidth
import com.example.gymtools.common.darkBackground

@Composable
fun SoundPlayerScreen(
    navController: NavHostController,
    modifier: Modifier = Modifier
) {

    val context = LocalContext.current
    var isPlaying by remember { mutableStateOf(false) }

    // MediaPlayer instance
    val mediaPlayer = remember { MediaPlayer.create(context, R.raw.music) }

    DisposableEffect(Unit) {
        onDispose {
            mediaPlayer.release()
        }
    }

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.back,
            description = "Back button",
            onClick = {
                mediaPlayer.pause()
                mediaPlayer.seekTo(0)
                navController.navigate("Settings")
            },
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        Column(
            horizontalAlignment = Alignment.CenterHorizontally,
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading("Audio player")

            Box(
                contentAlignment = Alignment.Center,
                modifier = Modifier
                    .fillMaxWidth()
                    .background(
                        color = darkBackground,
                        shape = RoundedCornerShape(16.dp)
                    )
                    .clickable {
                        if (mediaPlayer.isPlaying) {
                            mediaPlayer.pause()
                            isPlaying = false
                        } else {
                            mediaPlayer.start()
                            isPlaying = true
                        }
                    }
                    .padding(16.dp)

            ) {
                CustomText(if (isPlaying) "Pause" else "Play")
            }
            Spacer(modifier = Modifier.height(32.dp))

            // Reset Button
            Box(
                contentAlignment = Alignment.Center,
                modifier = Modifier
                    .fillMaxWidth()
                    .background(
                        color = darkBackground,
                        shape = RoundedCornerShape(16.dp)
                    )
                    .clickable {
                        mediaPlayer.seekTo(0)
                        if (mediaPlayer.isPlaying) {
                            mediaPlayer.start()
                            isPlaying = true
                        }
                    }
                    .padding(16.dp)

            ) {
                CustomText("Reset")
            }
        }
    }
}