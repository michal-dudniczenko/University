import { createContext, useContext } from "react";
import type { UserProfile } from "../types/UserProfile";

interface UtilsContextType {
    getAccessToken: () => string;
    updateAccessToken: (token: string) => void;
    refreshAccessToken: () => Promise<boolean>;
    logout: () => Promise<boolean>;
    getUserProfile: () => UserProfile | null;
    updateUserProfileData: () => Promise<boolean>;
}

export const UtilsContext = createContext<UtilsContextType | null>(null);

export const useUtils = () => {
    const ctx = useContext(UtilsContext);
    if (!ctx) {
        throw new Error("useUtilsmust be used within AuthProvider");
    }
    return ctx;
};
