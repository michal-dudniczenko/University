# Test Suite Classification — keep / remove / rename

## Context

The integration test project (`tests/Soundmates.IntegrationTests/`) currently holds **601 test
methods across 42 `*Tests.cs` files**. A large share were AI-generated and are noise: they re-verify
shared infrastructure (the JWT/cookie auth pipeline, the CSRF filter) identically on every endpoint,
re-test ASP.NET model-binding/routing, or merely assert that "a request reaches the handler". Example
of pure noise:

```csharp
[Fact]
public async Task GetMatches_CookieAuthReachesHandler_Returns200() { /* asserts 200, nothing else */ }
```

This document classifies **every** test method into **keep / remove / rename** so the suite can be
trimmed to behavior-focused tests with consistent `Method_Scenario_Result` names. Per your decisions:

- **Auth matrix** (token-forging + caller-state checks): consolidate into ONE shared auth-pipeline
  test class; keep only a single `NoCredentials_Returns401` smoke per endpoint to prove it isn't public.
- **Framework/model-binding tests**: remove, except where the app adds custom logic (upload
  content-type/extension/size validation).
- **Per-endpoint CSRF tests**: remove entirely — the central `Common/Csrf/CsrfTokenFilterTests.cs`
  already covers the filter.
- **Deliverable**: this classification document only. No test files are edited. (On approval I can
  optionally export this to a standalone repo doc.)

**Result: 360 KEEP (of which 47 RENAME), 241 REMOVE.**

---

## Classification rules

A method is **REMOVE** if its name matches any pattern below (these test shared infra / framework, and
are duplicated across ~20 endpoints):

| Bucket | Pattern (in method name) | Why |
|---|---|---|
| `RM-reach` | `*ReachesHandler*`, `*BothReachHandler`, `ValidCookieScheme`, `ValidJwtBearer`, `*AuthMode_ReachesHandler` | Only asserts the request reaches the handler — the happy-path test already does. |
| `RM-csrf` | contains `Csrf` (in any non-central file) — `CookieAuth*Csrf`, `Cookie*CsrfToken`, `JwtAuthSkipsCsrf`, `JwtRequest_SkipsCsrf`, `CookieMissingCsrf*` | Covered once by `CsrfTokenFilterTests`. |
| `RM-fw` | `GetToPostRoute`, `WrongHttpMethod`, `WrongMethod`, `UnknownRoute`, `NonIntegerLimit`, `MalformedJson`, `_EmptyBody_`, `WrongContentType` | ASP.NET routing/model-binding, not app logic. |
| `RM-auth` | token-forging `*_Returns401` (`Garbage*`, `Malformed Bearer`, `Expired*`, `WrongKey*`, `WrongIssuer*`, `WrongAudience*`, `InvalidAuthCookie`, `TokenFor{Deleted,Nonexistent}User`) **and** caller-state `*Caller_Returns401` / `{Unconfirmed,Deactivated,FirstLogin,Deleted}{User,Caller}_Returns401` | The shared auth pipeline / `GetAuthorizedUserAsync`, identical on every protected endpoint. |

**KEEP** = everything else, including: `NoCredentials_Returns401` (the per-endpoint "is it protected?"
smoke), all request-payload validation (`*_Returns422` from FluentValidation), pagination boundary
checks (`LimitZero/TooLarge/NegativeOffset/BothInvalid/ValidBoundaries`), business rules
(self-target, already-liked, no-match, receiver/target-state `*_Returns404`), DB side-effects, response
payload assertions, and the app's own media-upload validation.

**Two hand-overrides** the pattern matcher gets wrong (anonymous `/auth/refresh` lifecycle, not the
shared pipeline) — these are **KEEP**:
`Refresh_ExpiredToken_Returns401AndDeletesRow`, `Refresh_UnconfirmedUser_Returns401AndDeletesRow`.

### Rename convention

Strict three-part `Method_Scenario_Result`, where `Method` is the **full feature/endpoint name** and
`Scenario`/`Result` carry no internal underscores. This fixes ambiguous prefixes (`Delete_`, `Upload_`,
`GetPreview_`, `Deactivate_`, `ResendEmail_`) and collapses 4–5 segment names back to three parts.

---

## Per-file actions

Below, only **REMOVE** and **RENAME** rows are listed explicitly; every method not listed is **KEEP**.
Per-file totals are given so the split is complete.

### Auth (mostly unique behavior — keep)

- **ChangePasswordTests** (7): all KEEP.
- **ConfirmEmailTests** (6): all KEEP.
- **CsrfTokenTests** (1): KEEP (`/auth/csrf-token` endpoint behavior — distinct from the CSRF filter).
- **DeactivateAccountTests** (9): REMOVE `Deactivate_UnconfirmedCaller_Returns401`,
  `Deactivate_DeactivatedCaller_Returns401`, `Deactivate_TokenForNonexistentUser_Returns401`.
  RENAME the remaining 6 (prefix `Deactivate_` → `DeactivateAccount_`).
- **ForgotPasswordTests** (4): all KEEP.
- **LoginTests** (11): all KEEP.
- **LogoutTests** (3): all KEEP (`JwtAuth_Returns204AsNoop` is real behavior, not a reach test).
- **RefreshTests** (8): all KEEP (incl. the two hand-overridden lifecycle tests above).
- **RegisterTests** (11): all KEEP.
- **ResendEmailConfirmationTests** (4): RENAME all 4 (prefix `ResendEmail_` → `ResendEmailConfirmation_`).
- **ResetPasswordTests** (10): all KEEP.
- **RevokeAllTokensTests** (8): REMOVE `RevokeAllTokens_TokenForNonexistentUser_Returns401`,
  `RevokeAllTokens_UnconfirmedCaller_Returns401`, `RevokeAllTokens_DeactivatedCaller_Returns401`.
  (`FirstLoginUser_Returns204`, `NoExistingTokens_Returns204AsNoop`, `OnlyRemovesCallersTokens` KEEP.)
- **RevokeTokenTests** (4): all KEEP.

### Common/Csrf/CsrfTokenFilterTests (5): all KEEP — this is the central CSRF suite the per-endpoint copies defer to.

### Dictionaries (all KEEP)

`GetBandRoles`, `GetCities`, `GetCountries`, `GetGenders`, `GetTagCategories`, `GetTags` — all KEEP.
(`*_AnonymousRequest_ReturnsOk` asserts the endpoint opts out of the global auth fallback — app config,
worth keeping. `GetCities_CountryIdOmitted/NotAGuid_Returns400` = app GUID/required validation, keep.)

### Matching

- **CreateDislikeTests** (28): KEEP 12. REMOVE 16 → `RM-fw`: `EmptyBody_Returns400`,
  `MalformedJson_Returns400`, `WrongContentType_Returns415`, `GetToPostRoute_Returns405`; `RM-reach`:
  `JwtAndCookieBothReachHandler`; `RM-csrf`: the 4 `CookieAuth*Csrf*`/`JwtAuthSkipsCsrf`; `RM-auth`: the
  7 token/caller `*_Returns401` (`GarbageBearerToken`, `ExpiredToken`, `WrongKeyToken`,
  `InvalidAuthCookie`, `TokenForDeletedUser`, `UnconfirmedCaller`, `DeactivatedCaller`, `FirstLoginCaller`).
  (`NoCredentials_Returns401` KEEP; receiver-state `*Receiver_Returns404` KEEP.)
- **CreateLikeTests** (32): same shape — KEEP 15 (incl. `ReciprocalLike…SendsEvents`, `AfterMatch…400`,
  `AlreadyLiked/Disliked`, all `*Receiver*`, `NoCredentials`). REMOVE 17 (3 `RM-fw`, 1 `RM-reach`,
  4 `RM-csrf`, 9 `RM-auth`).
- **GetMatchPreferenceTests** (14): KEEP 4 (`NewlyOnboarded`, `WithStoredValues`, `NoPreferenceRow_500`,
  `NoCredentials`). REMOVE 10 (`RM-fw` `UnknownRoute_404`; `RM-reach` `CookieAuthReachesHandler`;
  `RM-auth` ×8).
- **GetMatchesTests** (24): KEEP 14 (happy/empty, media ordering, the 4 `OtherUser*_Excluded`, paging,
  pagination 422s + `ValidBoundaries`, `NoCredentials`). REMOVE 10 (`RM-fw` `NonIntegerLimit_400`;
  `RM-reach` `CookieAuthReachesHandler`; `RM-auth` ×8).
- **GetPotentialMatchesArtistsTests** (32): KEEP 22 (all filter/distance/paging/pagination behavior +
  `NoCredentials`). REMOVE 10 (`RM-fw` `NonIntegerLimit_400`; `RM-reach` `CookieAuthReachesHandler`;
  `RM-auth` ×8).
- **GetPotentialMatchesBandsTests** (31): KEEP 21, REMOVE 10 (same shape as Artists).
- **MatchExistsTests** (17): KEEP 8 (both-position truth, no-match, nonexistent, `NonGuidRouteParam_422`,
  `SelfTarget_400`, `NoCredentials`). REMOVE 9 (`RM-reach` ×1, `RM-auth` ×8).
- **UnmatchTests** (20): KEEP 7 (both-position delete, `MessagesSurvive`, `NonGuidRouteParam_422`,
  `SelfTarget_400`, `NoMatch_404`, `NoCredentials`). REMOVE 13 (`RM-reach` `JwtAndCookieBothReachHandler`;
  `RM-csrf` ×4; `RM-auth` ×8).
- **UpdateMatchPreferenceTests** (31): KEEP 16 (all field/tag persistence + the 422 validators +
  `NonexistentTagId_500`, `Skewing…Accepted`, `NoCredentials`). REMOVE 15 (`RM-fw` `MalformedJson_400`,
  `WrongContentType_415`, `WrongMethod_405`; `RM-reach` ×1; `RM-csrf` ×4; `RM-auth` ×7).

> Optional (not required by your decisions): the pagination 422/boundary tests are shared
> `PaginationValidator` behavior duplicated across the 4 list endpoints. They could likewise be
> consolidated into one pagination-validation suite, keeping a single boundary smoke per list endpoint.

### Messages

- **GetConversationTests** (30): KEEP 19 (ordering, tie-break, all pagination 422s + `ValidBoundaries`,
  paging, `OtherUser*_404`, no-match-still-history, self-id, inactive/unconfirmed-other-user-returns-
  history, `NoCredentials`). REMOVE 11 (`RM-fw` `NonIntegerLimit_400`; `RM-reach`
  `ValidCookieScheme_ReachesHandler`; `RM-auth` ×9).
- **GetConversationsPreviewTests** (15): KEEP 4 → all RENAME (prefix `GetPreview_` →
  `GetConversationsPreview_`): `…MultipleConversations…`, `…NoConversations_ReturnsEmptyList`,
  `…SameCreatedAt_PicksHigherIdAsLatest`, `…NoCredentials_Returns401`. REMOVE 11 (`RM-reach` ×1,
  `RM-auth` ×10).
- **SendMessageTests** (30): KEEP 12 (happy persist+notify, all `*_Returns422` content/receiver
  validators, `ContentExactlyMaxLength_Ok`, `ToSelf_400`, `Receiver{Nonexistent,Inactive,Unconfirmed}_404`,
  `NoMatch_401`, `NoCredentials`). REMOVE 18 (`RM-fw` `MalformedJson_400`, `WrongContentType_415`,
  `WrongHttpMethod_405`; `RM-csrf` ×5 incl. `CookieMissingCsrfAndInvalidBody…` & `JwtRequest_SkipsCsrf…`;
  `RM-auth` ×10).
- **ViewConversationTests** (25): KEEP 11 (flip-unseen+notify, only-incoming-flipped, no-unseen-still-ok,
  `OtherUserIdNotAGuid_422`, `SelfId_400`, `OtherUser{Nonexistent,Inactive,Unconfirmed}_404`,
  `NoMatch_401`, `OtherUserFirstLogin_ReturnsOk`, `NoCredentials`). REMOVE 14 (`RM-csrf` ×3, `RM-reach`
  ×1, `RM-auth` ×10).

### MusicSamples

- **DeleteMusicSampleTests** (22): KEEP 6 → all RENAME (`Delete_` → `DeleteMusicSample_`):
  - `OwnSample_ReturnsOkAndRowRemoved`
  - `SampleFileMissingOnDisk_ReturnsOkAndRemovesRow`
  - `NonGuidRouteParam_Returns422`
  - `NonexistentSampleId_Returns404`
  - `AnotherUsersSample_Returns401` (was `SampleBelongingToAnotherUser` — real ownership rule, keep)
  - `NoCredentials_Returns401`
  REMOVE 16 (`RM-csrf` ×4, `RM-reach` ×2 `Jwt/CookieAuthMode_ReachesHandler`, `RM-auth` ×10).
- **UploadMusicSampleTests** (27): KEEP 11 → all RENAME (`Upload_` → `UploadMusicSample_`):
  - `ValidMp3_ReturnsOkAndRowAdded`, `ValidMp4_ReturnsOkAndRowAdded`
  - `DisallowedContentTypeAllowedExtension_Returns400`
  - `AllowedContentTypeDisallowedExtension_Returns400`
  - `AudioMpegContentTypeMp4Extension_Returns400`
  - `VideoMp4ContentTypeMp3Extension_Returns400`
  - `FifthSampleWhenFourExist_ReturnsOkWithDisplayOrderFour`
  - `SixthSampleWhenFiveExist_Returns400`
  - `MissingFileField_Returns400` (app's multipart contract, keep)
  - `MultipleSamples_FilenameLowercasedAndDisplayOrderIncrements`
  - `NoCredentials_Returns401`
  REMOVE 16 (`RM-csrf` ×4, `RM-reach` ×2, `RM-auth` ×10).

### ProfilePictures

- **DeleteProfilePictureTests** (21): KEEP 6 → all RENAME (`Delete_` → `DeleteProfilePicture_`):
  - `OwnPicture_ReturnsOkAndRowRemoved`
  - `NonGuidRoute_Returns422`
  - `NonexistentPicture_Returns404`
  - `AnotherUsersPicture_Returns401`
  - `FileMissingOnDisk_ReturnsOkAndRemovesRow`
  - `NoCredentials_Returns401`
  REMOVE 15 (`RM-csrf` ×4, `RM-reach` ×1 `ValidJwtBearer_ReachesHandler`, `RM-auth` ×10).
- **UploadProfilePictureTests** (15): KEEP 11 → all RENAME (`Upload_` → `UploadProfilePicture_`):
  - `JpegFile_ReturnsOkWithRowAtDisplayOrderZero`
  - `JpgFile_ReturnsOk`
  - `DisallowedContentTypeAllowedExtension_Returns400`
  - `AllowedContentTypeDisallowedExtension_Returns400`
  - `OversizeFile_Returns400`, `ExactlyMaxSizeFile_ReturnsOk`
  - `WhenUserAlreadyHasFivePictures_Returns400` *(overlaps with the next — see note)*
  - `FifthAllowedSixthRejected_Returns200Then400` (was `FifthPicture_ReturnsOk_SixthPicture_Returns400`)
  - `MissingFileField_Returns400`
  - `MultipleUploads_ExtensionLowercasedAndDisplayOrderIncrements`
  - `NoCredentials_Returns401`
  REMOVE 4 (`RM-auth`: `TokenForDeletedUser`, `UnconfirmedEmailUser`, `DeactivatedUser`, `FirstLoginUser`).
  > Note: `WhenUserAlreadyHasFivePictures_Returns400` and `FifthAllowedSixthRejected…` overlap on the
  > 5-picture cap; consider dropping one.

### Reports

- **BlockUserTests** (8): all KEEP (admin role gate, `409` on already-inactive, own-account, then-login-403
  are all meaningful; `NonAdminCaller_403` is the role-authz check worth keeping per-endpoint).
- **ReportUserTests** (17): KEEP 13 (validators, at/over max-length, nonexistent/self → 200, HTML
  encoding, `NoCredentials`). REMOVE 4 (`RM-auth`: `DeletedCaller`, `UnconfirmedCaller`,
  `DeactivatedCaller`, `FirstLoginCaller`).

### Users

- **GetOtherUserProfileTests** (17): KEEP 12 (artist/band target shapes, all target-state `*_404`,
  `CallerRequestsOwnId_Returns200`, the two `IsBand*ButNo*Row_404`, `NonGuidUserId_422`, `NoCredentials`).
  REMOVE 5 (`RM-auth`: `DeletedCaller`, `UnconfirmedCaller`, `DeactivatedCaller`, `FirstLoginCaller`,
  and `FirstLoginCallerRequestsOwnId_Returns401` — the first-login gate fires in the shared pipeline
  before the own-id branch, so it's covered centrally).
- **GetSelfProfileTests** (9): KEEP 6 (onboarded artist/band, first-login base profile, the two
  `IsBand*ButNo*Row_404`, `NoCredentials`). REMOVE 3 (`RM-auth`: `DeletedCaller`, `UnconfirmedCaller`,
  `DeactivatedCaller`).
- **UpdateProfileTests** (29): KEEP 26 (all create/update/flag-flip behavior, boundary `Passes`
  theories, the 500-mapping FK/category/order-conflict cases, trimming, media-order subset/clear,
  `NoCredentials`). REMOVE 3 (`RM-auth`: `DeletedCaller`, `UnconfirmedCaller`, `DeactivatedCaller`).

---

## Recommended consolidation target (absorbs the 171 `RM-auth` removals)

Create one shared suite — e.g. `tests/Soundmates.IntegrationTests/Common/Auth/AuthPipelineTests.cs` —
that exercises the auth pipeline **once** against a representative protected endpoint, covering:

- token-forging rejections → 401: missing creds, garbage/malformed bearer, expired, wrong key, wrong
  issuer, wrong audience, invalid auth cookie, token-for-deleted-user;
- `GetAuthorizedUserAsync` caller-state rejections → 401: unconfirmed, deactivated, first-login;
- the `checkForFirstLogin: false` opt-out variant (so the `ChangePassword/Deactivate/Revoke*` endpoints
  that intentionally allow first-login callers are represented).

Each individual endpoint then retains only its `*_NoCredentials_Returns401` smoke (already KEEP above)
to prove it is not accidentally `AllowAnonymous`. The central CSRF suite already plays this role for CSRF.

---

## Verification

This task produces a classification only; no test files are modified. To validate the classification
against the live suite (read-only):

1. Confirm the method inventory still matches: `grep -rhoE "public (async Task|void) [A-Za-z0-9_]+" tests --include="*Tests.cs"` → 601 methods.
2. Spot-check the two hand-overrides by reading `Auth/RefreshTests.cs` — confirm `ExpiredToken`/
   `UnconfirmedUser` assert refresh-row deletion (domain behavior), not pipeline 401s.
3. If/when the removals are later applied, the suite must still build and the consolidated
   `AuthPipelineTests` must cover the matrix; run `dotnet test` to confirm green.
