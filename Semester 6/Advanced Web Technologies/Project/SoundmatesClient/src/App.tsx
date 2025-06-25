import { BrowserRouter, Routes, Route } from "react-router";

import RegisterForm from "./components/RegisterForm/RegisterForm";
import LoginForm from "./components/LoginForm/LoginForm";
import FirstLoginForm from "./components/FirstLoginForm/FirstLoginForm";
import MainWindow from "./components/MainWindow/MainWindow";
import { useState } from "react";
import type { UserProfile } from "./types/UserProfile";
import type { AccessTokenDto } from "./types/AccessTokenDto";
import { UtilsContext } from "./utils/UtilsContextType";
import SettingsScreen from "./components/SettingsScreen/SettingsScreen";
import EditProfile from "./components/EditProfile/EditProfile";
import ChangePassword from "./components/ChangePassword/ChangePassword";
import DeactivateAccount from "./components/DeactivateAccount/DeactivateAccount";

function App() {
    const [accessToken, setAccessToken] = useState("");
    const [userProfile, setUserProfile] = useState<UserProfile | null>(null);

    const getAccessToken = () => accessToken;

    const updateAccessToken = (token: string) => {
        setAccessToken(token);
    }

    const refreshAccessToken = async (): Promise<boolean> => {
        const url = "http://localhost:5000/users/refresh";

        const response = await fetch(url, {
            method: "POST",
            credentials: "include",
        });

        if (!response.ok) {
            return false;
        }

        const data = (await response.json()) as AccessTokenDto;

        setAccessToken(data.accessToken);
        return true;
    };

    const getUserProfile = () => userProfile;

    const updateUserProfileData = async () => {
        const url = "http://localhost:5000/users/profile";

        const request = new Request(url, {
            headers: {
                "Access-Token": accessToken,
            },
        });

        let response = await fetch(request);

        if (!response.ok) {
            const refreshed = await refreshAccessToken();
            if (!refreshed) {
                return false;
            }

            response = await fetch(request);

            if (!response.ok) {
                return false;
            }
        }

        const data = (await response.json()) as UserProfile;
        setUserProfile(data);
        return true;
    };

    const logout = async () => {
        const url = "http://localhost:5000/users/logout";

        const request = new Request(url, {
            method: "POST",
            headers: {
                "Access-Token": accessToken,
            },
        });

        let response = await fetch(request);

        if (!response.ok) {
            const refreshed = await refreshAccessToken();
            if (!refreshed) {
                return false;
            }

            response = await fetch(request);

            if (!response.ok) {
                return false;
            }
        }

        setAccessToken("");
        setUserProfile(null);
        return true;
    };

    return (
        <UtilsContext.Provider
            value={{
                getAccessToken,
                updateAccessToken,
                refreshAccessToken,
                logout,
                getUserProfile,
                updateUserProfileData,
            }}>
            <BrowserRouter>
                <Routes>
                    <Route
                        index
                        element={<MainWindow />}
                    />
                    <Route
                        path="register"
                        element={<RegisterForm />}
                    />
                    <Route
                        path="login"
                        element={<LoginForm />}
                    />
                    <Route
                        path="first-login"
                        element={<FirstLoginForm />}
                    />
                    <Route
                        path="settings"
                        element={<SettingsScreen />}
                    />
                    <Route
                        path="edit-profile"
                        element={<EditProfile />}
                    />
                    <Route
                        path="change-password"
                        element={<ChangePassword />}
                    />
                    <Route
                        path="deactivate"
                        element={<DeactivateAccount />}
                    />
                </Routes>
            </BrowserRouter>
        </UtilsContext.Provider>
    );
}

export default App;
