import { useCallback, useEffect, useRef, useState } from "react";
import type { OtherUserProfileDto } from "../../types/OtherUserProfileDto";
import type { UserProfile } from "../../types/UserProfile";
import { useUtils } from "../../utils/UtilsContextType";
import type { SwipeDto } from "../../types/SwipeDto";
import UserMatchCard from "../UserMatchCard/UserMatchCard";

function MatchingSection() {
    const [usersQueue, setUsersQueue] = useState<OtherUserProfileDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [noMoreMatches, setNoMoreMatches] = useState(false);
    const userProfileData = useRef<UserProfile | null>(null);
    const offset = useRef(0);

    const { getUserProfile, updateUserProfileData, getAccessToken, refreshAccessToken } = useUtils();

    const dequeue = () => {
        setUsersQueue((prev) => prev.slice(1));
    };

    const fetchMoreUsers = useCallback(async () => {
        const params = new URLSearchParams();
        params.append("limit", "20");
        params.append("offset", `${offset.current}`);

        const url = `http://localhost:5000/users?${params}`;

        const request = new Request(url, {
            headers: {
                "Access-Token": getAccessToken(),
            },
        });

        let response = await fetch(request);

        if (response.status === 401) {
            const refreshed = await refreshAccessToken();
            if (!refreshed) {
                return false;
            }
            response = await fetch(request);
        }

        if (!response.ok) {
            return false;
        }

        const data = (await response.json()) as OtherUserProfileDto[];

        if (data.length === 0) {
            setNoMoreMatches(true);
            return true;
        }

        setUsersQueue((prev) => {
            const existingIds = new Set(prev.map((u) => u.id));
            const newUnique = data.filter((u) => !existingIds.has(u.id));
            return [...prev, ...newUnique];
        });

        offset.current += 20;
        return true;
    }, [getAccessToken, refreshAccessToken]);

    const handleLike = async () => {
        const selfId = getUserProfile()?.id ?? "";
        const otherUserId = usersQueue[0].id;

        const url = "http://localhost:5000/matching/like";

        const dto: SwipeDto = {
            giverId: selfId,
            receiverId: otherUserId,
        };

        const request = new Request(url, {
            method: "POST",
            headers: {
                "Access-Token": getAccessToken(),
                "Content-Type": "application/json",
            },
            body: JSON.stringify(dto),
        });

        let response = await fetch(request);

        if (response.status === 401) {
            const refreshed = await refreshAccessToken();
            if (!refreshed) {
                return;
            }
            response = await fetch(request);
        }

        if (!response.ok) {
            return;
        }

        dequeue();
    };

    const handleDislike = async () => {
        const selfId = getUserProfile()?.id ?? "";
        const otherUserId = usersQueue[0].id;

        const url = "http://localhost:5000/matching/dislike";

        const dto: SwipeDto = {
            giverId: selfId,
            receiverId: otherUserId,
        };

        const request = new Request(url, {
            method: "POST",
            headers: {
                "Access-Token": getAccessToken(),
                "Content-Type": "application/json",
            },
            body: JSON.stringify(dto),
        });

        let response = await fetch(request);

        if (response.status === 401) {
            const refreshed = await refreshAccessToken();
            if (!refreshed) {
                return;
            }
            response = await fetch(request);
        }

        if (!response.ok) {
            return;
        }

        dequeue();
    };

    useEffect(() => {
        const initData = async () => {
            if (userProfileData === null) {
                let userProfile = getUserProfile();

                if (!userProfile) {
                    await updateUserProfileData();
                    userProfile = getUserProfile();
                }

                if (!userProfile) {
                    return;
                }
            }

            if (usersQueue.length === 0) {
                const loaded = await fetchMoreUsers();

                if (!loaded) {
                    return;
                }
            }

            setLoading(false);
        };

        initData();
    }, [fetchMoreUsers, getUserProfile, updateUserProfileData, usersQueue.length]);

    useEffect(() => {
        if (usersQueue.length === 0 && noMoreMatches) {
            setLoading(false); // just in case
        }

        if (usersQueue.length < 5 && !loading && !noMoreMatches) {
            fetchMoreUsers();
        }
    }, [usersQueue, noMoreMatches, loading, fetchMoreUsers]);

    if (noMoreMatches && usersQueue.length === 0) {
        return <div>Currently there are no more possible matches. Try again later.</div>;
    }

    if (loading || usersQueue.length === 0) {
        return <div>Loading...</div>;
    }

    return (
        <UserMatchCard
            userProfile={usersQueue[0]}
            handleLike={handleLike}
            handleDislike={handleDislike}
        />
    );
}

export default MatchingSection;
