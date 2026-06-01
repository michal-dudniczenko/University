# Integration Test Cleanup Plan — Soundmates.IntegrationTests

## Context

The integration test project contains **803 test methods across 42 `*Tests.cs` classes**. Most
were AI-generated and a large fraction is low-value noise: per-endpoint duplication of JWT/cookie
middleware rejection, framework HTTP plumbing (empty body, malformed JSON, wrong content-type,
wrong method, unknown route), CSRF round-trips, and near-duplicate validation tests. The coded,
snake_case naming (`Login_V2_invalid_email_returns_422`, `ReportUser_CCAUTH4_wrong_key_jwt_returns_401`)
is unreadable and non-standard.

**Goal:** cut the suite to tests that exercise *custom application logic* and *authorization guards*,
rename survivors to `MethodName_Scenario_ExpectedResult`, and surface a small set of genuinely
ambiguous tests for the team to decide.

This file is a decision document. No test code has been changed (plan mode).

### Classification rules applied
- **REMOVE** — verifies built-in middleware (JWT/cookie auth variants, rate limiting, antiforgery),
  third-party internals, asserts unexpected 500s, or framework HTTP plumbing.
- **KEEP** — verifies custom business logic, custom validation/domain error responses (422/4xx with
  app-defined bodies), or that a protected endpoint returns **401 without credentials** (one per endpoint).
- **FLAG** — genuinely ambiguous (framework vs. custom override) — left for the team.

### Three cross-cutting policy decisions (confirmed with the team)
1. **CSRF tests → consolidate.** The custom `ValidateCsrfTokenFilter` is real custom code, but the
   ~3–4 CSRF tests duplicated on *every* mutating endpoint are noise. **Remove all per-endpoint CSRF
   tests** and replace them with **one dedicated `Common/Csrf/CsrfTokenFilterTests.cs`** suite that
   exercises the filter's branches once (cookie-without-token → 400, cookie-with-invalid-token → 400,
   valid-token → pass, JWT-skips-CSRF → pass, and filter-runs-before-validation ordering).
2. **500-asserting tests → KEEP for now, just rename.** Several tests assert the app returns 500 where
   a 4xx domain error might be preferable. The team has decided to **keep these tests validating the
   current 500 behavior** (they document real, deliberate behavior for now) — only rename them to the
   convention. They are no longer flagged.
3. **"reaches handler" smoke tests → REMOVE as redundant.** `valid-jwt-reaches-handler` /
   `valid-cookie-reaches-handler` duplicate each endpoint's happy-path test and only prove auth-scheme wiring.

> **Team decisions folded in (resolving the previously-flagged §4 items):**
> - **500 tests** (4a) → keep + rename (policy #2 above).
> - **ReportUser HTML-in-email test** (4b) → source now HTML-encodes the email body; keep the test but
>   **update its assertion** to verify the content is *encoded* (not verbatim), and rename it.
> - **File-upload tests** (4c) → keep `Upload_MissingFileField_Returns400` as-is; change the oversize
>   test to expect **400 only** (drop the `Or413` branch) and rename accordingly.
> - **Invalid-route / wrong-HTTP-verb tests** (4d) → all REMOVE as framework logic (see §1e), including
>   the two `GetBandRoles` route tests.
> - **`GetSelfProfile_E3_email_is_returned_only_here`** (4e) → REMOVE (redundant with H1).
> - **Band-vs-Artist validator duplicates** (4f) → the shared rules are now abstracted into a single
>   reusable definition, so the band-side duplicate tests are **certain** removals (no per-rule verification needed).
> - Result: **section 4 is now empty** — no tests remain flagged.

---

## 1. Tests to remove — grouped by reason

These are highly patterned. The removals below are expressed as **patterns** (matching by method-name
suffix/marker) plus an explicit list of non-pattern removals. All counts are approximate; `[Theory]`
methods are counted once.

### 1a. Built-in authentication middleware (JWT / cookie rejection variants) — ~150 tests
For every protected endpoint, keep **only** the single `...NoCredentials_Returns401` /
`...CCAUTH1_no_token_returns_401` test. Remove all sibling variants that merely re-test the framework's
token validator:
- `*_GarbageBearer* / *_MalformedBearer* / *CCAUTH2*` (malformed token)
- `*_ExpiredJwt* / *CCAUTH3*` (expiry)
- `*_WrongKey* / *_WrongIssuer* / *_WrongAudience* / *CCAUTH4*` (signature/issuer/audience)
- `*_InvalidAuthCookie* / *CCAUTH5*` (cookie auth)

Affected across: every Auth class, all Matching classes, all Users classes, all Messages classes,
all MusicSamples/ProfilePictures classes, ReportUser.

### 1b. Auth-scheme "reaches handler" smoke tests (policy #3) — ~15 tests
Remove all `*CCAUTH6*`, `*_JwtAndCookieBothReachHandler`, `*_ValidCookieScheme_ReachesHandler`,
`*_JwtAuthMode_ReachesHandler`, `*_CookieAuthMode_ReachesHandler`, `Upload_ValidJwtBearer_ReachesHandler*`.
Reason: duplicates the endpoint's happy-path; proves only dual-scheme wiring.

### 1c. CSRF per-endpoint tests (policy #1) — ~45 tests
Remove **all** `*CCCSRF*`, `*_CookieAuth*Csrf*`, `*_CookieWithout/Invalid/ValidCsrf*`,
`*_JwtAuthSkipsCsrf*`, `*_JwtRequest_SkipsCsrf*`, `SendMessage_CookieMissingCsrfAndInvalidBody*`.
Replace with the single consolidated `CsrfTokenFilterTests.cs` (see §2 "to add").
Affected: ChangePassword, Deactivate, Logout, RevokeAllTokens, CsrfToken(H2), CreateLike, CreateDislike,
Unmatch, UpdateMatchPreference, SendMessage, ViewConversation, all 4 media classes, ReportUser, BlockUser.

### 1d. Rate-limiting middleware — ~10 tests
Remove all `*CCRL1*` (`..._eleventh_request_returns_429`) and `*CCRL2*` (`..._under_limit_unaffected`).
Affected: ConfirmEmail, ForgotPassword, Login, Logout, Refresh, Register, ResendEmailConfirmation,
ResetPassword, RevokeToken, RevokeAllTokens.

### 1e. Framework HTTP plumbing — ~70 tests
Remove:
- `*_EmptyBody_Returns400 / *CCVAL3*` (model binding)
- `*_MalformedJson_Returns400 / *CCVAL4*`
- `*_WrongContentType_Returns415 / *CCVAL5*`
- `*_NonIntegerLimit_Returns400` (query-param binding)
- `*_WrongMethod_Returns405 / *_GetToPostRoute_Returns405 / *_PostToGetRoute_Returns405 / *CCROUTE2*`
- `*_UnknownRoute_Returns404 / *CCROUTE1*`
Affected: most Auth classes, Create/Update Matching classes, UpdateProfile, SendMessage,
UpdateMatchPreference, all Dictionaries classes (route 404/405), CsrfToken.

> Per team decision (4d), this includes **all** invalid-route / wrong-verb tests with no exception —
> notably `GetBandRoles_UnknownRoute_Returns404` and `GetBandRoles_PostToGetRoute_Returns405` (whose
> bodies actually assert 401 via the global fallback policy; either way it's framework behavior → remove).

### 1f. Redundant near-duplicate validation tests — ~23 tests (UpdateProfile-heavy)
The AI generated the *same* validation rule twice (once for the Artist path, once for the Band path)
and added redundant length variants. The shared rules are now abstracted into a single reusable
definition (team confirmed), so band-side duplicates are **certain** removals. Keep one representative
per distinct rule; remove:
- `UpdateProfile_VA1_name_too_long`, `UpdateProfile_VB1_name_too_long`, `UpdateProfile_VB1_name_empty`
  duplicates where the rule is already covered on the artist side:
  `VB2_description_too_long`, `VB3_country_id_invalid`, `VB4_city_id_invalid`, `VB5_tags_ids_null`,
  `VB5_tags_element_invalid`, `VB6_music_samples_order_null`, `VB6_music_samples_element_invalid`,
  `VB7_profile_pictures_order_null`, `VB7_profile_pictures_element_invalid`, `VB10_member_name_too_long`.
- `GetSelfProfile_CCGA5_first_login_user_allowed`, `UpdateProfile_CCGA5_first_login_user_allowed`,
  `UpdateProfile_E1_first_login_user_is_allowed` (duplicate the H-series happy paths).
- `GetSelfProfile_E3_email_is_returned_only_here` (redundant with `H1`, which already asserts `body.Email`).
- `Logout_CCAUTH1_no_credentials_returns_401` (duplicate of `Logout_E1_unauthenticated_returns_401`).
- `ProfilePicture Upload_BothContentTypeAndExtensionDisallowed_Returns400` (same code path as the
  disallowed-content-type test).
- `ConfirmEmail_E3_successful_confirm_does_not_hit_defensive_500` (asserts absence of a 500 — no value).

### 1g. CsrfToken endpoint anonymous/route tests — 3 tests
`CsrfToken_E1_works_anonymously`, `CsrfToken_CCROUTE1/2` (AllowAnonymous + framework routing).
Keep `CsrfToken_H1` (custom cache headers + structured response body).

---

## 2. Tests to keep as-is

These files already follow the naming convention and their kept tests need **no rename**. Listed by
class (only the kept tests survive; the removals from §1 still apply within each):

- **Matching** — `CreateLikeTests`, `CreateDislikeTests`, `GetMatchesTests`, `GetMatchPreferenceTests`,
  `GetPotentialMatchesArtistsTests`, `GetPotentialMatchesBandsTests`, `MatchExistsTests`,
  `UnmatchTests`, `UpdateMatchPreferenceTests`. Strong custom-logic coverage (Haversine distance,
  age/gender/tag/band-size filters, reciprocal match creation, both-position match queries,
  reaction dedup, pagination 422s). Keep all non-removed tests verbatim.
- **Messages** — `GetConversationTests`, `GetConversationsPreviewTests`, `SendMessageTests`,
  `ViewConversationTests`. Custom ordering/tie-break, only-matches-can-message (401), mark-as-viewed,
  SignalR payloads.
- **MusicSamples / ProfilePictures** — `UploadMusicSampleTests`, `DeleteMusicSampleTests`,
  `UploadProfilePictureTests`, `DeleteProfilePictureTests`. File-type/size/count limits, on-disk
  persistence/deletion, ownership enforcement, DisplayOrder.
- **Dictionaries** — keep one `..._Returns...OrderedByName` (query+shape) and one
  `..._AnonymousRequest_ReturnsOk` (confirms intended `AllowAnonymous`) per class; plus
  `GetCities` filter/empty-list/nonexistent-country behaviors.

**Optional (non-blocking) rename:** media tests use the shortened prefix `Upload_`/`Delete_`. To match
the convention's "real method name," they could become `UploadMusicSample_…`, `DeleteProfilePicture_…`,
etc. Low priority — readable as-is within their class.

### To ADD (replaces removed CSRF coverage)
`tests/Soundmates.IntegrationTests/Common/Csrf/CsrfTokenFilterTests.cs` exercising the custom
`ValidateCsrfTokenFilter` once against a single representative mutating endpoint:
`CsrfFilter_CookieAuthWithoutToken_Returns400`, `CsrfFilter_CookieAuthWithInvalidToken_Returns400`,
`CsrfFilter_CookieAuthWithValidToken_PassesThrough`, `CsrfFilter_JwtAuth_SkipsCheck`,
`CsrfFilter_RunsBeforeValidation_Returns400NotValidationProblem`.

---

## 3. Tests to keep with rename (old → new)

Transformation: drop the code marker (`H1_`, `V1_`, `F2_`, `E1_`, `CCAUTH1_`, `CCGA2_`), convert
snake_case to a readable `MethodName_Scenario_ExpectedResult`. Only the **kept** tests are listed.

### Auth/ChangePasswordTests
- `ChangePassword_H1_jwt_correct_old_password_returns_204_and_revokes_tokens` → `ChangePassword_WithValidOldPassword_Returns204AndRevokesRefreshTokens`
- `ChangePassword_V1_empty_old_password_returns_422` → `ChangePassword_EmptyOldPassword_Returns422`
- `ChangePassword_V2_invalid_new_password_returns_422` → `ChangePassword_InvalidNewPassword_Returns422`
- `ChangePassword_F2_wrong_old_password_returns_401` → `ChangePassword_WrongOldPassword_Returns401`
- `ChangePassword_E1_first_login_user_can_change_password` → `ChangePassword_FirstLoginUser_Returns204`
- `ChangePassword_E2_deactivated_user_jwt_still_reaches_handler` → `ChangePassword_DeactivatedUserWithValidJwt_Returns204`
- `ChangePassword_CCAUTH1_no_token_returns_401` → `ChangePassword_NoCredentials_Returns401`

### Auth/ConfirmEmailTests
- `ConfirmEmail_H1_valid_token_creates_user_and_match_preference` → `ConfirmEmail_ValidToken_CreatesUserWithMatchPreferenceAndDeletesPending`
- `ConfirmEmail_V1_empty_token_returns_422` → `ConfirmEmail_EmptyToken_Returns422`
- `ConfirmEmail_F1_unknown_token_returns_400` → `ConfirmEmail_UnknownToken_Returns400`
- `ConfirmEmail_F2_expired_token_returns_400` → `ConfirmEmail_ExpiredToken_Returns400`
- `ConfirmEmail_E1_token_is_consumed_second_use_returns_400` → `ConfirmEmail_ReusedToken_Returns400`
- `ConfirmEmail_E2_email_cannot_be_reregistered_after_confirm` → `ConfirmEmail_ThenRegisterSameEmail_Returns422`

### Auth/CsrfTokenTests
- `CsrfToken_H1_returns_token_cookie_and_no_cache_headers` → `CsrfToken_Get_ReturnsTokenWithCookieAndNoCacheHeaders`

### Auth/DeactivateAccountTests
- `Deactivate_H1_correct_password_deactivates_and_revokes_tokens` → `Deactivate_CorrectPassword_DeactivatesUserAndRevokesTokens`
- `Deactivate_V1_empty_password_returns_422` → `Deactivate_EmptyPassword_Returns422`
- `Deactivate_F1_wrong_password_returns_401` → `Deactivate_WrongPassword_Returns401`
- `Deactivate_E1_first_login_user_allowed` → `Deactivate_FirstLoginUser_Returns204`
- `Deactivate_E2_after_deactivation_login_returns_403` → `Deactivate_ThenLogin_Returns403`
- `Deactivate_CCGA2_unconfirmed_user_returns_401` → `Deactivate_UnconfirmedCaller_Returns401`
- `Deactivate_CCGA3_inactive_user_returns_401` → `Deactivate_DeactivatedCaller_Returns401`
- `Deactivate_CCGA1_missing_user_returns_401` → `Deactivate_TokenForNonexistentUser_Returns401`
- `Deactivate_CCAUTH1_no_token_returns_401` → `Deactivate_NoCredentials_Returns401`

### Auth/ForgotPasswordTests
- `ForgotPassword_H1_confirmed_user_sends_reset_email_with_token` → `ForgotPassword_ConfirmedUser_SendsResetEmail`
- `ForgotPassword_F1_unknown_email_returns_204_no_email` → `ForgotPassword_UnknownEmail_Returns204AndSendsNoEmail`
- `ForgotPassword_F2_unconfirmed_user_returns_204_no_email` → `ForgotPassword_UnconfirmedUser_Returns204AndSendsNoEmail`
- `ForgotPassword_V1_invalid_email_returns_422` → `ForgotPassword_InvalidEmail_Returns422`

### Auth/LoginTests
- `Login_H1_no_cookies_returns_tokens_and_persists_refresh_token` → `Login_ValidCredentialsNoCookies_ReturnsTokensAndPersistsRefreshToken`
- `Login_H2_with_cookies_returns_empty_body_and_sets_auth_cookie` → `Login_ValidCredentialsWithCookies_SetsAuthCookieAndReturnsEmptyBody`
- `Login_V1_missing_useCookies_returns_400_validation_problem` → `Login_MissingUseCookiesParam_Returns400`
- `Login_V2_invalid_email_returns_422` → `Login_InvalidEmail_Returns422`
- `Login_V3_empty_password_returns_422` → `Login_EmptyPassword_Returns422`
- `Login_F1_email_not_found_returns_401` → `Login_NonexistentEmail_Returns401`
- `Login_F2_deactivated_user_returns_403` → `Login_DeactivatedUser_Returns403`
- `Login_F3_wrong_password_returns_401` → `Login_WrongPassword_Returns401`
- `Login_F4_lockout_after_five_failures_returns_423` → `Login_AfterFiveFailedAttempts_Returns423`
- `Login_F5_unconfirmed_email_returns_401` → `Login_UnconfirmedEmail_Returns401`
- `Login_E2_each_login_issues_new_refresh_token` → `Login_MultipleLogins_EachIssuesNewRefreshToken`

### Auth/LogoutTests
- `Logout_H1_cookie_session_returns_204_and_clears_auth_cookie` → `Logout_CookieSession_Returns204AndClearsAuthCookie`
- `Logout_E1_unauthenticated_returns_401` → `Logout_NoCredentials_Returns401`
- `Logout_E2_jwt_auth_returns_204_noop` → `Logout_JwtAuth_Returns204AsNoop`

### Auth/RefreshTests
- `Refresh_H1_valid_token_returns_tokens_and_rotates` → `Refresh_ValidToken_ReturnsNewTokensAndRotates`
- `Refresh_V1_empty_token_returns_422` → `Refresh_EmptyToken_Returns422`
- `Refresh_F1_unknown_token_returns_401` → `Refresh_UnknownToken_Returns401`
- `Refresh_F2_expired_token_returns_401_and_deletes_row` → `Refresh_ExpiredToken_Returns401AndDeletesRow`
- `Refresh_F3_inactive_user_returns_401_and_deletes_row` → `Refresh_InactiveUser_Returns401AndDeletesRow`
- `Refresh_F4_unconfirmed_user_returns_401_and_deletes_row` → `Refresh_UnconfirmedUser_Returns401AndDeletesRow`
- `Refresh_E1_old_token_after_rotation_returns_401` → `Refresh_ReusedTokenAfterRotation_Returns401`
- `Refresh_E2_new_access_token_carries_expected_claims` → `Refresh_NewAccessToken_ContainsRequiredClaims`

### Auth/RegisterTests
- `Register_H1_creates_pending_registration_and_sends_email` → `Register_ValidRequest_CreatesPendingRowAndSendsConfirmationEmail`
- `Register_H2_purges_expired_pending_rows` → `Register_PurgesExpiredPendingRows`
- `Register_H3_same_unconfirmed_email_creates_second_pending_row` → `Register_SameUnconfirmedEmail_CreatesSecondPendingRow`
- `Register_V1_empty_email_returns_422` → `Register_EmptyEmail_Returns422`
- `Register_V1_email_too_long_returns_422` → `Register_EmailTooLong_Returns422`
- `Register_V1_invalid_email_format_returns_422` → `Register_InvalidEmailFormat_Returns422`
- `Register_V2_email_of_confirmed_user_returns_422` → `Register_AlreadyConfirmedEmail_Returns422`
- `Register_V3_invalid_password_returns_422` → `Register_InvalidPassword_Returns422`
- `Register_V3_boundary_password_passes` → `Register_BoundaryValidPassword_Returns204`
- `Register_V4_cascade_stop_returns_only_first_email_error` → `Register_EmptyEmail_ReturnsSingleEmailError`
- `Register_E1_password_never_stored_in_plaintext_body_empty` → `Register_PasswordStoredHashed_NotInPlaintext`

### Auth/ResendEmailConfirmationTests
- `ResendEmail_H1_existing_pending_rotates_token_and_sends_email` → `ResendEmail_ExistingPendingRow_RotatesTokenAndSendsEmail`
- `ResendEmail_F1_no_pending_row_returns_204_no_email` → `ResendEmail_NoPendingRow_Returns204AndSendsNoEmail`
- `ResendEmail_V1_invalid_email_returns_422` → `ResendEmail_InvalidEmail_Returns422`
- `ResendEmail_V1_email_too_long_returns_422` → `ResendEmail_EmailTooLong_Returns422`

### Auth/ResetPasswordTests
- `ResetPassword_H1_valid_token_resets_password_and_revokes_refresh_tokens` → `ResetPassword_ValidToken_ResetsPasswordRevokesTokensAndAllowsLogin`
- `ResetPassword_V1_invalid_email_returns_422` → `ResetPassword_InvalidEmail_Returns422`
- `ResetPassword_V2_empty_token_returns_422` → `ResetPassword_EmptyToken_Returns422`
- `ResetPassword_V3_invalid_new_password_returns_422` → `ResetPassword_InvalidNewPassword_Returns422`
- `ResetPassword_F1_unknown_email_returns_400` → `ResetPassword_UnknownEmail_Returns400`
- `ResetPassword_F1_unconfirmed_user_returns_400` → `ResetPassword_UnconfirmedUser_Returns400`
- `ResetPassword_F2_valid_base64url_but_invalid_token_returns_400` → `ResetPassword_ValidBase64ButInvalidIdentityToken_Returns400`
- `ResetPassword_E2_old_refresh_token_unusable_after_reset` → `ResetPassword_AfterReset_OldRefreshTokenIsRevoked`
- `ResetPassword_E3_reusing_old_password_is_allowed` → `ResetPassword_SamePasswordAsOld_Returns204`
- `ResetPassword_E1_non_base64url_token_throws_returns_500_with_traceId` → `ResetPassword_NonBase64UrlToken_Returns500`  *(kept per policy #2 — validates current 500 behavior)*

### Auth/RevokeAllTokensTests
- `RevokeAll_H1_deletes_all_tokens` → `RevokeAllTokens_MultipleTokens_DeletesAll`
- `RevokeAll_E1_first_login_user_allowed` → `RevokeAllTokens_FirstLoginUser_Returns204`
- `RevokeAll_E2_zero_tokens_returns_204` → `RevokeAllTokens_NoExistingTokens_Returns204AsNoop`
- `RevokeAll_E3_only_callers_tokens_removed` → `RevokeAllTokens_OnlyRemovesCallersTokens`
- `RevokeAll_CCAUTH1_no_credentials_returns_401` → `RevokeAllTokens_NoCredentials_Returns401`
- `RevokeAll_CCGA1_token_for_nonexistent_user_returns_401` → `RevokeAllTokens_TokenForNonexistentUser_Returns401`
- `RevokeAll_CCGA2_unconfirmed_user_returns_401` → `RevokeAllTokens_UnconfirmedCaller_Returns401`
- `RevokeAll_CCGA3_deactivated_user_returns_401` → `RevokeAllTokens_DeactivatedCaller_Returns401`

### Auth/RevokeTokenTests
- `RevokeToken_H1_existing_token_returns_204_and_deletes_row` → `RevokeToken_ExistingToken_Returns204DeletesRowAndBlocksRefresh`
- `RevokeToken_V1_empty_token_returns_422` → `RevokeToken_EmptyToken_Returns422`
- `RevokeToken_E1_unknown_token_returns_204_noop` → `RevokeToken_UnknownToken_Returns204AsIdempotentNoop`
- `RevokeToken_E2_only_supplied_token_affected` → `RevokeToken_OnlySuppliedTokenIsRevoked`

### Users/GetOtherUserProfileTests
- `..._H1_artist_target_returns_artist_profile_without_email` → `GetOtherUserProfile_ArtistTarget_ReturnsProfileWithoutEmail`
- `..._H2_band_target_returns_band_members` → `GetOtherUserProfile_BandTarget_ReturnsBandMembers`
- `..._V1_non_guid_route_returns_422` → `GetOtherUserProfile_NonGuidUserId_Returns422`
- `..._F1_nonexistent_target_returns_404` → `GetOtherUserProfile_TargetNotFound_Returns404`
- `..._F1_inactive_target_returns_404` → `GetOtherUserProfile_InactiveTarget_Returns404`
- `..._F1_first_login_target_returns_404` → `GetOtherUserProfile_FirstLoginTarget_Returns404`
- `..._F1_unconfirmed_target_returns_404` → `GetOtherUserProfile_UnconfirmedTarget_Returns404`
- `..._F1_is_band_null_target_returns_404` → `GetOtherUserProfile_IsBandNullTarget_Returns404`
- `..._E1_onboarded_caller_own_id_returns_200` → `GetOtherUserProfile_CallerRequestsOwnId_Returns200`
- `..._E1_first_login_caller_own_id_returns_404` → `GetOtherUserProfile_FirstLoginCallerRequestsOwnId_Returns401`  *(current name says 404 but asserts 401 — fix in rename)*
- `..._E2_is_band_true_but_missing_band_row_returns_404` → `GetOtherUserProfile_IsBandTrueButNoBandRow_Returns404`
- `..._E2_is_band_false_but_missing_artist_row_returns_404` → `GetOtherUserProfile_IsBandFalseButNoArtistRow_Returns404`
- `..._CCAUTH1_no_credentials_returns_401` → `GetOtherUserProfile_NoCredentials_Returns401`
- `..._CCGA1_deleted_caller_returns_401` → `GetOtherUserProfile_DeletedCaller_Returns401`
- `..._CCGA2_unconfirmed_caller_returns_401` → `GetOtherUserProfile_UnconfirmedCaller_Returns401`
- `..._CCGA3_deactivated_caller_returns_401` → `GetOtherUserProfile_DeactivatedCaller_Returns401`
- `..._CCGA4_first_login_caller_returns_401` → `GetOtherUserProfile_FirstLoginCaller_Returns401`

### Users/GetSelfProfileTests
- `..._H1_onboarded_artist_returns_full_profile_with_email_and_media` → `GetSelfProfile_OnboardedArtist_ReturnsFullProfileWithEmailAndOrderedMedia`
- `..._H2_onboarded_band_returns_band_members` → `GetSelfProfile_OnboardedBand_ReturnsBandMembersWithEmail`
- `..._H3_first_login_user_returns_base_profile_with_nulls_and_empty_collections` → `GetSelfProfile_FirstLoginUser_ReturnsBaseProfileWithNullsAndEmptyCollections`
- `..._E1_is_band_true_but_no_band_row_returns_404` → `GetSelfProfile_IsBandTrueButNoBandRow_Returns404`
- `..._E2_is_band_false_but_no_artist_row_returns_404` → `GetSelfProfile_IsBandFalseButNoArtistRow_Returns404`
- `..._CCAUTH1_no_credentials_returns_401` → `GetSelfProfile_NoCredentials_Returns401`
- `..._CCGA1_deleted_user_returns_401` → `GetSelfProfile_DeletedCaller_Returns401`
- `..._CCGA2_unconfirmed_email_returns_401` → `GetSelfProfile_UnconfirmedCaller_Returns401`
- `..._CCGA3_deactivated_user_returns_401` → `GetSelfProfile_DeactivatedCaller_Returns401`

### Users/UpdateProfileTests (kept tests)
- `..._H1_first_time_artist_creates_artist_row_and_sets_flags` → `UpdateProfile_FirstTimeArtist_CreatesArtistRowAndFlipsFlags`
- `..._H2_first_time_band_creates_band_with_ordered_members` → `UpdateProfile_FirstTimeBand_CreatesBandRowWithOrderedMembers`
- `..._H3_update_existing_artist_updates_row_in_place` → `UpdateProfile_ExistingArtist_UpdatesRowInPlace`
- `..._H4_update_existing_band_clears_and_readds_members_in_order` → `UpdateProfile_ExistingBand_ClearsAndReaddsMembersInOrder`
- `..._H5_first_call_flips_is_first_login_and_unlocks_guarded_endpoints` → `UpdateProfile_FirstCall_FlipsIsFirstLoginAndUnlocksGuardedEndpoints`
- `..._VA1_name_empty_returns_422` → `UpdateProfile_ArtistNameEmpty_Returns422`
- `..._VA2_description_too_long_returns_422` → `UpdateProfile_ArtistDescriptionTooLong_Returns422`
- `..._VA3_country_id_invalid_returns_422` → `UpdateProfile_ArtistCountryIdInvalid_Returns422`
- `..._VA4_city_id_invalid_returns_422` → `UpdateProfile_ArtistCityIdInvalid_Returns422`
- `..._VA5_tags_ids_null_returns_422` → `UpdateProfile_ArtistTagsIdsNull_Returns422`
- `..._VA5_tags_element_invalid_returns_422` → `UpdateProfile_ArtistTagsElementInvalid_Returns422`
- `..._VA6_music_samples_order_null_returns_422` → `UpdateProfile_ArtistMusicSamplesOrderNull_Returns422`
- `..._VA6_music_samples_element_invalid_returns_422` → `UpdateProfile_ArtistMusicSamplesElementInvalid_Returns422`
- `..._VA7_profile_pictures_order_null_returns_422` → `UpdateProfile_ArtistProfilePicturesOrderNull_Returns422`
- `..._VA7_profile_pictures_element_invalid_returns_422` → `UpdateProfile_ArtistProfilePicturesElementInvalid_Returns422`
- `..._VA8_birth_date_invalid_returns_422` → `UpdateProfile_ArtistBirthDateInvalid_Returns422`
- `..._VA8_birth_date_min_boundary_1900_passes` → `UpdateProfile_ArtistBirthDateMinBoundary_Passes`
- `..._VA8_birth_date_today_boundary_passes` → `UpdateProfile_ArtistBirthDateTodayBoundary_Passes`
- `..._VA9_gender_id_invalid_returns_422` → `UpdateProfile_ArtistGenderIdInvalid_Returns422`
- `..._VB1_name_empty_returns_422` → `UpdateProfile_BandNameEmpty_Returns422`
- `..._VB8_band_members_null_returns_422` → `UpdateProfile_BandMembersNull_Returns422`
- `..._VB9_band_members_count_100_fails_returns_422` → `UpdateProfile_BandMembersCountAtMax_Returns422`
- `..._VB9_band_members_count_99_passes` → `UpdateProfile_BandMembersCount99_Passes`
- `..._VB9_band_members_count_0_passes` → `UpdateProfile_BandMembersCountZero_Passes`
- `..._VB10_member_name_empty_returns_422` → `UpdateProfile_BandMemberNameEmpty_Returns422`
- `..._VB10_member_age_out_of_range_returns_422` → `UpdateProfile_BandMemberAgeOutOfRange_Returns422`
- `..._VB10_member_band_role_invalid_returns_422` → `UpdateProfile_BandMemberBandRoleIdInvalid_Returns422`
- `..._VB10_member_age_boundaries_pass` → `UpdateProfile_BandMemberAgeBoundaries_Pass`
- `..._E2_switching_artist_to_band_leaves_old_artist_row` → `UpdateProfile_SwitchArtistToBand_LeavesOldArtistRow`
- `..._E3_subset_order_drops_omitted_media` → `UpdateProfile_SubsetMediaOrder_DropsOmittedItems`
- `..._E4_name_description_and_member_name_are_trimmed` → `UpdateProfile_NameDescriptionAndMemberName_AreTrimmed`
- `..._E5_empty_order_lists_clear_media` → `UpdateProfile_EmptyOrderLists_ClearAllMedia`
- `..._CCAUTH1_no_credentials_returns_401` → `UpdateProfile_NoCredentials_Returns401`
- `..._CCGA1_deleted_user_returns_401` → `UpdateProfile_DeletedCaller_Returns401`
- `..._CCGA2_unconfirmed_email_returns_401` → `UpdateProfile_UnconfirmedCaller_Returns401`
- `..._CCGA3_deactivated_user_returns_401` → `UpdateProfile_DeactivatedCaller_Returns401`
- `..._B1_unknown_discriminator_returns_400` → `UpdateProfile_UnknownUserType_Returns400`  *(custom handler fallthrough returns 400 — keep)*

UpdateProfile 500-asserting tests — **kept per policy #2** (validate current 500 behavior), renamed:
- `..._F1a_nonexistent_tag_returns_500` → `UpdateProfile_NonexistentTagId_Returns500`
- `..._F1b_artist_request_with_band_category_tag_returns_500` → `UpdateProfile_ArtistWithBandCategoryTag_Returns500`
- `..._F1b_band_request_with_artist_category_tag_returns_500` → `UpdateProfile_BandWithArtistCategoryTag_Returns500`
- `..._F2_music_samples_order_duplicate_returns_500` → `UpdateProfile_DuplicateMusicSampleOrder_Returns500`
- `..._F3_music_samples_order_non_owned_returns_500` → `UpdateProfile_NonOwnedMusicSampleOrder_Returns500`
- `..._F4_profile_pictures_order_duplicate_returns_500` → `UpdateProfile_DuplicateProfilePictureOrder_Returns500`
- `..._F4_profile_pictures_order_non_owned_returns_500` → `UpdateProfile_NonOwnedProfilePictureOrder_Returns500`
- `..._F5_nonexistent_country_fk_returns_500` → `UpdateProfile_NonexistentCountryFk_Returns500`
- `..._F5_nonexistent_gender_fk_returns_500` → `UpdateProfile_NonexistentGenderFk_Returns500`
- `..._F5_nonexistent_band_role_fk_returns_500` → `UpdateProfile_NonexistentBandRoleFk_Returns500`

### Reports/ReportUserTests
- `ReportUser_H1_sends_moderation_email_with_all_fields` → `ReportUser_ValidRequest_SendsModerationEmailWithAllFields`
- `ReportUser_V1_empty_reported_user_id_returns_422` → `ReportUser_EmptyReportedUserId_Returns422`
- `ReportUser_V1_non_guid_reported_user_id_returns_422` → `ReportUser_NonGuidReportedUserId_Returns422`
- `ReportUser_V2_empty_reason_returns_422` → `ReportUser_EmptyReason_Returns422`
- `ReportUser_V2_reason_over_200_returns_422` → `ReportUser_ReasonOverMaxLength_Returns422`
- `ReportUser_V2_reason_exactly_200_passes` → `ReportUser_ReasonAtMaxLength_Returns200`
- `ReportUser_V3_empty_description_returns_422` → `ReportUser_EmptyDescription_Returns422`
- `ReportUser_V3_description_over_1000_returns_422` → `ReportUser_DescriptionOverMaxLength_Returns422`
- `ReportUser_V3_description_exactly_1000_passes` → `ReportUser_DescriptionAtMaxLength_Returns200`
- `ReportUser_E1_nonexistent_reported_user_returns_200` → `ReportUser_NonexistentReportedUser_Returns200`  *(documents a known no-existence-check gap)*
- `ReportUser_E2_reporting_self_returns_200` → `ReportUser_ReportSelf_Returns200`  *(documents a known no-self-report-guard gap)*
- `ReportUser_E3_html_content_in_fields_appears_verbatim_in_email_body` → `ReportUser_HtmlInFields_IsEncodedInEmailBody`  *(4b fixed in source — **update the assertion** to verify the email body HTML-encodes user input instead of asserting verbatim HTML)*
- `ReportUser_CCAUTH1_no_credentials_returns_401` → `ReportUser_NoCredentials_Returns401`
- `ReportUser_CCGA1_deleted_user_returns_401` → `ReportUser_DeletedCaller_Returns401`
- `ReportUser_CCGA2_unconfirmed_email_returns_401` → `ReportUser_UnconfirmedCaller_Returns401`
- `ReportUser_CCGA3_deactivated_user_returns_401` → `ReportUser_DeactivatedCaller_Returns401`
- `ReportUser_CCGA4_first_login_user_returns_401` → `ReportUser_FirstLoginCaller_Returns401`

### Reports/BlockUserTests
- `BlockUser_H1_admin_blocks_active_user_returns_204_and_deactivates` → `BlockUser_AdminBlocksActiveUser_Returns204AndDeactivates`
- `BlockUser_A1_unauthenticated_returns_401` → `BlockUser_NoCredentials_Returns401`
- `BlockUser_A2_non_admin_returns_403` → `BlockUser_NonAdminCaller_Returns403`  *(custom `RequireAdmin` policy — keep)*
- `BlockUser_F1_non_guid_route_returns_404_not_422` → `BlockUser_NonGuidRouteSegment_Returns404`
- `BlockUser_F2_nonexistent_user_returns_404` → `BlockUser_NonexistentUser_Returns404`
- `BlockUser_F3_already_inactive_target_returns_409` → `BlockUser_AlreadyInactiveTarget_Returns409`
- `BlockUser_E1_admin_blocking_own_account_returns_204_gap` → `BlockUser_AdminBlocksOwnAccount_Returns204`  *(documents no-self-block gap)*
- `BlockUser_E2_after_block_target_login_returns_403` → `BlockUser_ThenTargetLogin_Returns403`

### Dictionaries (minor cleanups of the kept happy-path names)
- `GetCountries_ReturnsOk_WithAllCountriesOrderedByName` → `GetCountries_ReturnsAllCountriesOrderedByName`
- `GetGenders_ReturnsOk_WithAllGendersOrderedByName` → `GetGenders_ReturnsAllGendersOrderedByName`
- `GetTags_ReturnsOk_WithAllTagsOrderedByName` → `GetTags_ReturnsAllTagsOrderedByName`
- `GetTagCategories_ReturnsOk_WithAllCategoriesOrderedByName` → `GetTagCategories_ReturnsAllCategoriesOrderedByName`
- `GetBandRoles_ReturnsOk_WithAllBandRolesOrderedByName` → `GetBandRoles_ReturnsAllBandRolesOrderedByName`
- `Get*_WithNoCredentials_ReturnsOk` → `Get*_AnonymousRequest_ReturnsOk` (all six dictionary classes)

### ProfilePictures/UploadProfilePictureTests (4c — modify + rename)
- `Upload_OversizeFile_Returns400Or413` → `Upload_OversizeFile_Returns400` — **change the assertion to
  expect 400 only** (drop the `Or413` acceptance; the app's `ApplicationConstants` size check is the
  enforcer). `Upload_MissingFileField_Returns400` and `Upload_ExactlyMaxSizeFile_ReturnsOk` stay as-is.
  *(MusicSamples has no oversize test — nothing to change there.)*

---

## 4. Tests flagged for manual review

**None.** All previously-flagged items were resolved by team decisions (folded into §1 and §3):
4a → kept + renamed (policy #2); 4b → kept, assertion updated for the source fix; 4c → kept / modified
to expect 400; 4d → removed as framework logic; 4e → removed as redundant; 4f → removed (duplication
now abstracted, so removal is certain).

---

## 5. Summary (approximate counts; `[Theory]` counted once)

The 500-asserting tests and the previously-flagged items are now folded into Keep/Remove, so the Flag
column is zero. The ~14 kept 500-tests shifted into Keep; the GetBandRoles route tests + `GetSelfProfile_E3`
shifted into Remove.

| Domain | Total | Keep (as-is) | Keep (rename) | Remove | Flag |
|---|---:|---:|---:|---:|---:|
| Auth (13 classes) | 181 | 0 | ~84 | ~97 | 0 |
| Matching (9) | 230 | ~143 | 0 | ~87 | 0 |
| Users (3) | 119 | 0 | ~67 | ~52 | 0 |
| Messages (4) | 102 | ~60 | 0 | ~42 | 0 |
| MusicSamples + ProfilePictures (4) | 97 | ~47 | 0 | ~50 | 0 |
| Reports + Dictionaries (8) | 73 | ~12 | ~22 | ~39 | 0 |
| **Total** | **~803** | **~262** | **~173** | **~367** | **0** |

- **Keep:** ~435 (262 as-is + 173 renamed) — ~54%
- **Remove:** ~367 — ~46% (auth/csrf/plumbing/rate-limit duplication + validation dupes)
- **Flag:** 0 — all resolved by team decisions
- **Add:** 1 new file (`Common/Csrf/CsrfTokenFilterTests.cs`, ~5 tests) replacing ~45 removed CSRF tests.

Counts are estimates from per-domain analysis; the exact final numbers depend on `[Theory]` row
handling. Removals are dominated by repeated middleware/plumbing patterns (§1a–§1e), which is why the
cut is large but low-risk.

---

## Execution & verification

1. Apply removals by pattern (§1), then the explicit non-pattern removals; delete now-empty
   `*TestConstants`/`Contracts` only if fully unreferenced.
2. Apply renames (§3) file-by-file; update any `[Theory]`/`nameof` references.
3. Apply the two small assertion changes:
   - `ReportUser` HTML test → assert the email body HTML-**encodes** user input (source fixed, 4b).
   - `UploadProfilePicture` oversize test → assert **400 only** (drop the `Or413` branch, 4c).
4. Add `CsrfTokenFilterTests.cs` (§2).
5. **Verify:** Run `dotnet build` and verify that it produces no errors. Spot-check that each protected
   endpoint still has exactly one `*_NoCredentials_Returns401` test and that Matching/Messages
   custom-logic coverage is untouched.
```
