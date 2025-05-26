package com.example.befit.constants

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.compose.ui.graphics.Color
import com.example.befit.R

object AppThemes {
    const val DARK = "Dark"
    const val LIGHT = "Light"
    const val OCEAN = "Ocean"
    const val FOREST = "Forest"
    const val BARBIE = "Barbie"
    const val DEFAULT = DARK

    val values = listOf(
        DARK, LIGHT, OCEAN, FOREST, BARBIE
    )
}

var appTheme by mutableStateOf<String>(AppThemes.DARK)

object Themes {
    private val currentSet: ColorSet
        get() = Colorsets.allColorSets[appTheme] ?: Colorsets.allColorSets[AppThemes.DEFAULT]!!

    val PRIMARY: Color get() = currentSet.PRIMARY
    val ON_PRIMARY: Color get() = currentSet.ON_PRIMARY
    val SECONDARY: Color get() = currentSet.SECONDARY
    val ON_SECONDARY: Color get() = currentSet.ON_SECONDARY
    val BACKGROUND: Color get() = currentSet.BACKGROUND
    val ON_BACKGROUND: Color get() = currentSet.ON_BACKGROUND
    val EDIT_COLOR: Color get() = currentSet.EDIT_COLOR
    val ON_EDIT: Color get() = currentSet.ON_EDIT
    val DELETE_CANCEL_COLOR: Color get() = currentSet.DELETE_CANCEL_COLOR
    val ON_DELETE_CANCEL: Color get() = currentSet.ON_DELETE_CANCEL
    val ADD_CONFIRM_COLOR: Color get() = currentSet.ADD_CONFIRM_COLOR
    val ON_ADD_CONFIRM: Color get() = currentSet.ON_ADD_CONFIRM

    val ABC_ON_PRIMARY: Int get() = currentSet.ABC_ON_PRIMARY
    val CALENDAR_ON_PRIMARY: Int get() = currentSet.CALENDAR_ON_PRIMARY
    val CLOCK_ON_PRIMARY: Int get() = currentSet.CLOCK_ON_PRIMARY
    val NUMBERS_ON_PRIMARY: Int get() = currentSet.NUMBERS_ON_PRIMARY

    val ADD_ON_SECONDARY: Int get() = currentSet.ADD_ON_SECONDARY
    val BACK_ON_SECONDARY: Int get() = currentSet.BACK_ON_SECONDARY
    val EDIT_ON_SECONDARY: Int get() = currentSet.EDIT_ON_SECONDARY
    val SETTINGS_ON_SECONDARY: Int get() = currentSet.SETTINGS_ON_SECONDARY
    val VIDEO_ON_SECONDARY: Int get() = currentSet.VIDEO_ON_SECONDARY

    val EDIT_ON_EDIT: Int get() = currentSet.EDIT_ON_EDIT

    val CANCEL_ON_DELETE_CANCEL: Int get() = currentSet.CANCEL_ON_DELETE_CANCEL
    val TRASH_ON_DELETE_CANCEL: Int get() = currentSet.TRASH_ON_DELETE_CANCEL
    val CHECK_ON_ADD_CONFIRM: Int get() = currentSet.CHECK_ON_ADD_CONFIRM

    val DUMBBELL_PRIMARY: Int get() = currentSet.DUMBBELL_PRIMARY
    val DUMBBELL_ON_PRIMARY: Int get() = currentSet.DUMBBELL_ON_PRIMARY
    val STOPWATCH_PRIMARY: Int get() = currentSet.STOPWATCH_PRIMARY
    val STOPWATCH_ON_PRIMARY: Int get() = currentSet.STOPWATCH_ON_PRIMARY
    val HEALTH_PRIMARY: Int get() = currentSet.HEALTH_PRIMARY
    val HEALTH_ON_PRIMARY: Int get() = currentSet.HEALTH_ON_PRIMARY
    val SETTINGS_PRIMARY: Int get() = currentSet.SETTINGS_PRIMARY
    val SETTINGS_ON_PRIMARY: Int get() = currentSet.SETTINGS_ON_PRIMARY

    val PLAY_ON_SECONDARY: Int get() = currentSet.PLAY_ON_SECONDARY
    val PAUSE_ON_SECONDARY: Int get() = currentSet.PAUSE_ON_SECONDARY
    val RESET_ON_SECONDARY: Int get() = currentSet.RESET_ON_SECONDARY

    val SOUND_ON_SECONDARY: Int get() = currentSet.SOUND_ON_SECONDARY
    val SOUND_MUTED_ON_SECONDARY: Int get() = currentSet.SOUND_MUTED_ON_SECONDARY
}

data class ColorSet (
    val PRIMARY: Color,
    val ON_PRIMARY: Color,
    val SECONDARY: Color,
    val ON_SECONDARY: Color,
    val BACKGROUND: Color,
    val ON_BACKGROUND: Color,
    val EDIT_COLOR: Color,
    val ON_EDIT: Color,
    val DELETE_CANCEL_COLOR: Color,
    val ON_DELETE_CANCEL: Color,
    val ADD_CONFIRM_COLOR: Color,
    val ON_ADD_CONFIRM: Color,

    val ABC_ON_PRIMARY: Int,
    val CALENDAR_ON_PRIMARY: Int,
    val CLOCK_ON_PRIMARY: Int,
    val NUMBERS_ON_PRIMARY: Int,

    val ADD_ON_SECONDARY: Int,
    val BACK_ON_SECONDARY: Int,
    val EDIT_ON_SECONDARY: Int,
    val SETTINGS_ON_SECONDARY: Int,
    val VIDEO_ON_SECONDARY: Int,

    val EDIT_ON_EDIT: Int,

    val CANCEL_ON_DELETE_CANCEL: Int,
    val TRASH_ON_DELETE_CANCEL: Int,
    val CHECK_ON_ADD_CONFIRM: Int,

    val DUMBBELL_PRIMARY: Int,
    val DUMBBELL_ON_PRIMARY: Int,
    val STOPWATCH_PRIMARY: Int,
    val STOPWATCH_ON_PRIMARY: Int,
    val HEALTH_PRIMARY: Int,
    val HEALTH_ON_PRIMARY: Int,
    val SETTINGS_PRIMARY: Int,
    val SETTINGS_ON_PRIMARY: Int,

    val PLAY_ON_SECONDARY: Int,
    val PAUSE_ON_SECONDARY: Int,
    val RESET_ON_SECONDARY: Int,

    val SOUND_ON_SECONDARY: Int,
    val SOUND_MUTED_ON_SECONDARY: Int
)

object Colorsets {
    val allColorSets: Map<String, ColorSet> = mapOf(
        AppThemes.DARK to ColorSet(
            PRIMARY = Color(15, 15, 15),
            ON_PRIMARY = Color(200, 200, 200),
            SECONDARY = Color(215, 215, 215, 255),
            ON_SECONDARY = Color(21, 21, 21, 255),
            BACKGROUND = Color(35, 35, 35),
            ON_BACKGROUND = Color(229, 229, 229, 255),
            EDIT_COLOR = Color(37, 133, 171, 255),
            ON_EDIT = Color(210, 210, 210, 255),
            DELETE_CANCEL_COLOR = Color(230, 84, 87, 255),
            ON_DELETE_CANCEL = Color(210, 210, 210, 255),
            ADD_CONFIRM_COLOR = Color(76, 155, 76, 255),
            ON_ADD_CONFIRM = Color(210, 210, 210, 255),
            ABC_ON_PRIMARY = R.drawable.dark_abc_on_primary,
            CALENDAR_ON_PRIMARY = R.drawable.dark_calendar_on_primary,
            CLOCK_ON_PRIMARY = R.drawable.dark_clock_on_primary,
            NUMBERS_ON_PRIMARY = R.drawable.dark_numbers_on_primary,
            ADD_ON_SECONDARY = R.drawable.dark_add_on_secondary,
            BACK_ON_SECONDARY = R.drawable.dark_back_on_secondary,
            EDIT_ON_SECONDARY = R.drawable.dark_edit_on_secondary,
            SETTINGS_ON_SECONDARY = R.drawable.dark_settings_on_secondary,
            EDIT_ON_EDIT = R.drawable.dark_edit_on_edit,
            CANCEL_ON_DELETE_CANCEL = R.drawable.dark_cancel_on_delete_cancel,
            TRASH_ON_DELETE_CANCEL = R.drawable.dark_trash_on_delete_cancel,
            CHECK_ON_ADD_CONFIRM = R.drawable.dark_check_on_add_confirm,
            DUMBBELL_PRIMARY = R.drawable.dark_dumbbell_primary,
            DUMBBELL_ON_PRIMARY = R.drawable.dark_dumbbell_on_primary,
            STOPWATCH_PRIMARY = R.drawable.dark_stopwatch_primary,
            STOPWATCH_ON_PRIMARY = R.drawable.dark_stopwatch_on_primary,
            HEALTH_PRIMARY = R.drawable.dark_health_primary,
            HEALTH_ON_PRIMARY = R.drawable.dark_health_on_primary,
            SETTINGS_PRIMARY = R.drawable.dark_settings_primary,
            SETTINGS_ON_PRIMARY = R.drawable.dark_settings_on_primary,
            PLAY_ON_SECONDARY = R.drawable.dark_play_on_secondary,
            PAUSE_ON_SECONDARY = R.drawable.dark_pause_on_secondary,
            RESET_ON_SECONDARY = R.drawable.dark_reset_on_secondary,
            VIDEO_ON_SECONDARY = R.drawable.dark_video_on_secondary,
            SOUND_ON_SECONDARY = R.drawable.dark_sound_on_secondary,
            SOUND_MUTED_ON_SECONDARY = R.drawable.dark_sound_muted_on_secondary
        ),
        AppThemes.LIGHT to ColorSet(
            PRIMARY = Color(59, 59, 59, 255),
            ON_PRIMARY = Color(234, 234, 234, 255),
            SECONDARY = Color(14, 14, 14, 255),
            ON_SECONDARY = Color(238, 238, 238, 255),
            BACKGROUND = Color(206, 206, 206, 255),
            ON_BACKGROUND = Color(25, 25, 25),
            EDIT_COLOR = Color(0, 120, 170, 255),
            ON_EDIT = Color(255, 255, 255, 255),
            DELETE_CANCEL_COLOR = Color(220, 60, 60, 255),
            ON_DELETE_CANCEL = Color(255, 255, 255, 255),
            ADD_CONFIRM_COLOR = Color(76, 175, 80, 255),
            ON_ADD_CONFIRM = Color(255, 255, 255, 255),
            ABC_ON_PRIMARY = R.drawable.dark_abc_on_primary,
            CALENDAR_ON_PRIMARY = R.drawable.dark_calendar_on_primary,
            CLOCK_ON_PRIMARY = R.drawable.dark_clock_on_primary,
            NUMBERS_ON_PRIMARY = R.drawable.dark_numbers_on_primary,
            ADD_ON_SECONDARY = R.drawable.dark_add_on_secondary,
            BACK_ON_SECONDARY = R.drawable.dark_back_on_secondary,
            EDIT_ON_SECONDARY = R.drawable.dark_edit_on_secondary,
            SETTINGS_ON_SECONDARY = R.drawable.dark_settings_on_secondary,
            EDIT_ON_EDIT = R.drawable.dark_edit_on_edit,
            CANCEL_ON_DELETE_CANCEL = R.drawable.dark_cancel_on_delete_cancel,
            TRASH_ON_DELETE_CANCEL = R.drawable.dark_trash_on_delete_cancel,
            CHECK_ON_ADD_CONFIRM = R.drawable.dark_check_on_add_confirm,
            DUMBBELL_PRIMARY = R.drawable.dark_dumbbell_primary,
            DUMBBELL_ON_PRIMARY = R.drawable.dark_dumbbell_on_primary,
            STOPWATCH_PRIMARY = R.drawable.dark_stopwatch_primary,
            STOPWATCH_ON_PRIMARY = R.drawable.dark_stopwatch_on_primary,
            HEALTH_PRIMARY = R.drawable.dark_health_primary,
            HEALTH_ON_PRIMARY = R.drawable.dark_health_on_primary,
            SETTINGS_PRIMARY = R.drawable.dark_settings_primary,
            SETTINGS_ON_PRIMARY = R.drawable.dark_settings_on_primary,
            PLAY_ON_SECONDARY = R.drawable.dark_play_on_secondary,
            PAUSE_ON_SECONDARY = R.drawable.dark_pause_on_secondary,
            RESET_ON_SECONDARY = R.drawable.dark_reset_on_secondary,
            VIDEO_ON_SECONDARY = R.drawable.dark_video_on_secondary,
            SOUND_ON_SECONDARY = R.drawable.dark_sound_on_secondary,
            SOUND_MUTED_ON_SECONDARY = R.drawable.dark_sound_muted_on_secondary
        ),
        AppThemes.OCEAN to ColorSet(
            PRIMARY = Color(0, 48, 73),
            ON_PRIMARY = Color(220, 240, 255),
            SECONDARY = Color(200, 200, 200),
            ON_SECONDARY = Color(15, 15, 15),
            BACKGROUND = Color(0, 70, 112),
            ON_BACKGROUND = Color(35, 35, 35),
            EDIT_COLOR = Color(0, 145, 207, 255),
            ON_EDIT = Color(240, 250, 255, 255),
            DELETE_CANCEL_COLOR = Color(255, 82, 82, 255),
            ON_DELETE_CANCEL = Color(255, 255, 255, 255),
            ADD_CONFIRM_COLOR = Color(0, 200, 255, 255),
            ON_ADD_CONFIRM = Color(0, 40, 60, 255),
            ABC_ON_PRIMARY = R.drawable.dark_abc_on_primary,
            CALENDAR_ON_PRIMARY = R.drawable.dark_calendar_on_primary,
            CLOCK_ON_PRIMARY = R.drawable.dark_clock_on_primary,
            NUMBERS_ON_PRIMARY = R.drawable.dark_numbers_on_primary,
            ADD_ON_SECONDARY = R.drawable.dark_add_on_secondary,
            BACK_ON_SECONDARY = R.drawable.dark_back_on_secondary,
            EDIT_ON_SECONDARY = R.drawable.dark_edit_on_secondary,
            SETTINGS_ON_SECONDARY = R.drawable.dark_settings_on_secondary,
            EDIT_ON_EDIT = R.drawable.dark_edit_on_edit,
            CANCEL_ON_DELETE_CANCEL = R.drawable.dark_cancel_on_delete_cancel,
            TRASH_ON_DELETE_CANCEL = R.drawable.dark_trash_on_delete_cancel,
            CHECK_ON_ADD_CONFIRM = R.drawable.dark_check_on_add_confirm,
            DUMBBELL_PRIMARY = R.drawable.dark_dumbbell_primary,
            DUMBBELL_ON_PRIMARY = R.drawable.dark_dumbbell_on_primary,
            STOPWATCH_PRIMARY = R.drawable.dark_stopwatch_primary,
            STOPWATCH_ON_PRIMARY = R.drawable.dark_stopwatch_on_primary,
            HEALTH_PRIMARY = R.drawable.dark_health_primary,
            HEALTH_ON_PRIMARY = R.drawable.dark_health_on_primary,
            SETTINGS_PRIMARY = R.drawable.dark_settings_primary,
            SETTINGS_ON_PRIMARY = R.drawable.dark_settings_on_primary,
            PLAY_ON_SECONDARY = R.drawable.dark_play_on_secondary,
            PAUSE_ON_SECONDARY = R.drawable.dark_pause_on_secondary,
            RESET_ON_SECONDARY = R.drawable.dark_reset_on_secondary,
            VIDEO_ON_SECONDARY = R.drawable.dark_video_on_secondary,
            SOUND_ON_SECONDARY = R.drawable.dark_sound_on_secondary,
            SOUND_MUTED_ON_SECONDARY = R.drawable.dark_sound_muted_on_secondary
        ),
        AppThemes.FOREST to ColorSet(
            PRIMARY = Color(34, 45, 34),
            ON_PRIMARY = Color(210, 240, 210),
            SECONDARY = Color(200, 200, 200),
            ON_SECONDARY = Color(15, 15, 15),
            BACKGROUND = Color(44, 66, 44),
            ON_BACKGROUND = Color(35, 35, 35),
            EDIT_COLOR = Color(88, 144, 88, 255),
            ON_EDIT = Color(240, 255, 240, 255),
            DELETE_CANCEL_COLOR = Color(153, 27, 27, 255),
            ON_DELETE_CANCEL = Color(250, 250, 250, 255),
            ADD_CONFIRM_COLOR = Color(76, 175, 80, 255),
            ON_ADD_CONFIRM = Color(255, 255, 255, 255),
            ABC_ON_PRIMARY = R.drawable.dark_abc_on_primary,
            CALENDAR_ON_PRIMARY = R.drawable.dark_calendar_on_primary,
            CLOCK_ON_PRIMARY = R.drawable.dark_clock_on_primary,
            NUMBERS_ON_PRIMARY = R.drawable.dark_numbers_on_primary,
            ADD_ON_SECONDARY = R.drawable.dark_add_on_secondary,
            BACK_ON_SECONDARY = R.drawable.dark_back_on_secondary,
            EDIT_ON_SECONDARY = R.drawable.dark_edit_on_secondary,
            SETTINGS_ON_SECONDARY = R.drawable.dark_settings_on_secondary,
            EDIT_ON_EDIT = R.drawable.dark_edit_on_edit,
            CANCEL_ON_DELETE_CANCEL = R.drawable.dark_cancel_on_delete_cancel,
            TRASH_ON_DELETE_CANCEL = R.drawable.dark_trash_on_delete_cancel,
            CHECK_ON_ADD_CONFIRM = R.drawable.dark_check_on_add_confirm,
            DUMBBELL_PRIMARY = R.drawable.dark_dumbbell_primary,
            DUMBBELL_ON_PRIMARY = R.drawable.dark_dumbbell_on_primary,
            STOPWATCH_PRIMARY = R.drawable.dark_stopwatch_primary,
            STOPWATCH_ON_PRIMARY = R.drawable.dark_stopwatch_on_primary,
            HEALTH_PRIMARY = R.drawable.dark_health_primary,
            HEALTH_ON_PRIMARY = R.drawable.dark_health_on_primary,
            SETTINGS_PRIMARY = R.drawable.dark_settings_primary,
            SETTINGS_ON_PRIMARY = R.drawable.dark_settings_on_primary,
            PLAY_ON_SECONDARY = R.drawable.dark_play_on_secondary,
            PAUSE_ON_SECONDARY = R.drawable.dark_pause_on_secondary,
            RESET_ON_SECONDARY = R.drawable.dark_reset_on_secondary,
            VIDEO_ON_SECONDARY = R.drawable.dark_video_on_secondary,
            SOUND_ON_SECONDARY = R.drawable.dark_sound_on_secondary,
            SOUND_MUTED_ON_SECONDARY = R.drawable.dark_sound_muted_on_secondary
        ),
        AppThemes.BARBIE to ColorSet(
            PRIMARY = Color(248, 84, 148, 255),
            ON_PRIMARY = Color(255, 225, 241, 255),
            SECONDARY = Color(224, 33, 138, 255),
            ON_SECONDARY = Color(255, 236, 246, 255),
            BACKGROUND = Color(232, 187, 203, 255),
            ON_BACKGROUND = Color(129, 44, 81, 255),
            EDIT_COLOR = Color(28, 144, 192, 255),
            ON_EDIT = Color(255, 255, 255, 255),
            DELETE_CANCEL_COLOR = Color(220, 60, 60, 255),
            ON_DELETE_CANCEL = Color(255, 255, 255, 255),
            ADD_CONFIRM_COLOR = Color(76, 175, 80, 255),
            ON_ADD_CONFIRM = Color(255, 255, 255, 255),
            ABC_ON_PRIMARY = R.drawable.dark_abc_on_primary,
            CALENDAR_ON_PRIMARY = R.drawable.dark_calendar_on_primary,
            CLOCK_ON_PRIMARY = R.drawable.dark_clock_on_primary,
            NUMBERS_ON_PRIMARY = R.drawable.dark_numbers_on_primary,
            ADD_ON_SECONDARY = R.drawable.dark_add_on_secondary,
            BACK_ON_SECONDARY = R.drawable.dark_back_on_secondary,
            EDIT_ON_SECONDARY = R.drawable.dark_edit_on_secondary,
            SETTINGS_ON_SECONDARY = R.drawable.dark_settings_on_secondary,
            EDIT_ON_EDIT = R.drawable.dark_edit_on_edit,
            CANCEL_ON_DELETE_CANCEL = R.drawable.dark_cancel_on_delete_cancel,
            TRASH_ON_DELETE_CANCEL = R.drawable.dark_trash_on_delete_cancel,
            CHECK_ON_ADD_CONFIRM = R.drawable.dark_check_on_add_confirm,
            DUMBBELL_PRIMARY = R.drawable.dark_dumbbell_primary,
            DUMBBELL_ON_PRIMARY = R.drawable.dark_dumbbell_on_primary,
            STOPWATCH_PRIMARY = R.drawable.dark_stopwatch_primary,
            STOPWATCH_ON_PRIMARY = R.drawable.dark_stopwatch_on_primary,
            HEALTH_PRIMARY = R.drawable.dark_health_primary,
            HEALTH_ON_PRIMARY = R.drawable.dark_health_on_primary,
            SETTINGS_PRIMARY = R.drawable.dark_settings_primary,
            SETTINGS_ON_PRIMARY = R.drawable.dark_settings_on_primary,
            PLAY_ON_SECONDARY = R.drawable.dark_play_on_secondary,
            PAUSE_ON_SECONDARY = R.drawable.dark_pause_on_secondary,
            RESET_ON_SECONDARY = R.drawable.dark_reset_on_secondary,
            VIDEO_ON_SECONDARY = R.drawable.dark_video_on_secondary,
            SOUND_ON_SECONDARY = R.drawable.dark_sound_on_secondary,
            SOUND_MUTED_ON_SECONDARY = R.drawable.dark_sound_muted_on_secondary
        )
    )
}