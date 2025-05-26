package com.example.befit.trainingprograms

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.offset
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.runtime.Composable
import androidx.compose.runtime.DisposableEffect
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import androidx.compose.ui.viewinterop.AndroidView
import androidx.core.net.toUri
import androidx.media3.common.MediaItem
import androidx.media3.common.Player
import androidx.media3.exoplayer.ExoPlayer
import androidx.media3.ui.PlayerView
import com.example.befit.common.CustomFloatingButton
import com.example.befit.common.Heading
import com.example.befit.constants.Themes
import com.example.befit.constants.adaptiveWidth

@Composable
fun ExerciseVideoScreen(
    exerciseId: Int,
    viewModel: TrainingProgramsViewModel,
    onBack: () -> Unit,
    modifier: Modifier = Modifier
) {
    val exercises by viewModel.exercises
    val exercise = exercises.find { it.id == exerciseId }

    val context = LocalContext.current

    val player = remember {
        ExoPlayer.Builder(context).build().apply {
            playWhenReady = true
            repeatMode = Player.REPEAT_MODE_ALL
            volume = 0f
        }
    }

    DisposableEffect(Unit) {
        onDispose {
            player.release()
        }
    }

    LaunchedEffect(exercise?.videoId) {
        exercise?.let {
            val videoUri = ("android.resource://${context.packageName}/${it.videoId}").toUri()
            player.setMediaItem(MediaItem.fromUri(videoUri))
            player.prepare()
        }
    }

    var isMuted by remember { mutableStateOf(true) }

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = Themes.BACK_ON_SECONDARY,
            description = "Back button",
            onClick = onBack,
            modifier = Modifier
                .align(Alignment.BottomStart)
                .offset(x = adaptiveWidth(32).dp, y = adaptiveWidth(-32).dp)
        )
        CustomFloatingButton(
            icon = if (isMuted) Themes.SOUND_MUTED_ON_SECONDARY else Themes.SOUND_ON_SECONDARY,
            description = "Mute button",
            onClick = {
                isMuted = !isMuted
                player.volume = if (isMuted) 0f else 1f
            },
            modifier = Modifier
                .align(Alignment.BottomEnd)
                .offset(x = adaptiveWidth(-32).dp, y = adaptiveWidth(-32).dp)
        )
        if (exercise == null) {
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
            modifier = modifier
                .fillMaxWidth(0.8f)
                .fillMaxHeight(0.9f)
                .align(Alignment.Center)
        ) {
            Heading(exercise!!.name)
            Column(
                verticalArrangement = Arrangement.Bottom,
                horizontalAlignment = Alignment.CenterHorizontally,
                modifier = Modifier
                    .fillMaxWidth(0.95f)
                    .fillMaxHeight(0.45f)
                    .verticalScroll(rememberScrollState())
            ) {
                AndroidView(
                    factory = { context ->
                        PlayerView(context).also {
                            it.player = player
                        }
                    },
                    modifier = Modifier
                        .fillMaxWidth()
                        .clip(RoundedCornerShape(20.dp))
                        .aspectRatio(16 / 9f)
                )
            }
        }
    }
}
