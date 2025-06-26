import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { useUtils } from "../../utils/UtilsContextType";
import MatchingSection from "../MatchingSection/MatchingSection";

function MainWindow() {
    const navigate = useNavigate();
    const { logout, getUserProfile, updateUserProfileData } = useUtils();

    const [loading, setLoading] = useState(true);
    const [logoutMessage, setLogoutMessage] = useState("");

    const handleLogout = async () => {
        setLogoutMessage("");
        const success = logout();
        if (!success) {
            setLogoutMessage("Something went wrong... Sign out failed.");
            return;
        }
        navigate("/login");
    };

    const openMatches = () => {
        navigate("/matches");
    }

    const openSettings = () => {
        navigate("/settings");
    }

    useEffect(() => {
        const checkAndNavigate = async () => {
            let userProfile = getUserProfile();

            if (!userProfile) {
                await updateUserProfileData();
                userProfile = getUserProfile();
            }

            if (!userProfile) {
                return;
            }

            if (userProfile.isFirstLogin) {
                navigate("/first-login", {
                    state: { userId: userProfile.id },
                });
                return;
            }

            setLoading(false);
        };

        checkAndNavigate();
    }, [getUserProfile, updateUserProfileData, navigate]);

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <div className="m-3">
            <div className="d-flex justify-content-between">
                <h1>Hi, {getUserProfile()?.name}!</h1>
                <div>
                    <button
                        onClick={openMatches}
                        className="btn btn-success me-3">
                        Matches
                    </button>
                    <button
                        onClick={openSettings}
                        className="btn btn-primary me-3">
                        Settings
                    </button>
                    <button
                        onClick={handleLogout}
                        className="btn btn-danger">
                        Sign out
                    </button>
                </div>
            </div>

            {logoutMessage && <h3>{logoutMessage}</h3>}

            <MatchingSection />
        </div>
    );
}

export default MainWindow;
