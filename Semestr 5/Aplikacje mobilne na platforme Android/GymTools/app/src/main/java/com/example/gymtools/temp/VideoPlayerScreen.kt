package com.example.gymtools.temp

import android.net.Uri
import android.widget.MediaController
import android.widget.VideoView
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.offset
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import androidx.compose.ui.viewinterop.AndroidView
import androidx.navigation.NavHostController
import com.example.gymtools.R
import com.example.gymtools.common.CustomFloatingButton
import com.example.gymtools.common.Heading
import com.example.gymtools.common.adaptiveWidth

@Composable
fun VideoPlayerScreen(
    navController: NavHostController,
    modifier: Modifier = Modifier
) {
    val context = LocalContext.current

    // Video URI from res/raw
    val videoUri = remember {
        Uri.parse("android.resource://${context.packageName}/${R.raw.cat}")
    }

    // Remember VideoView
    var videoView: VideoView? by remember { mutableStateOf(null) }

    Box(
        modifier = modifier
            .fillMaxSize()
    ) {
        CustomFloatingButton(
            icon = R.drawable.back,
            description = "Back button",
            onClick = {
                videoView?.pause()
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
            Heading("Video player")

            AndroidView(
                factory = { ctx ->
                    VideoView(ctx).apply {
                        setVideoURI(videoUri)

                        // MediaController for playback controls
                        val mediaController = MediaController(ctx)
                        mediaController.setAnchorView(this)
                        setMediaController(mediaController)

                        // Assign the VideoView instance
                        videoView = this
                    }
                },
                modifier = Modifier
                    .fillMaxHeight(0.8f)
            )
        }
    }
}