<script setup lang="ts">
import { computed } from 'vue';
import { initData, useSignal, type User } from '@telegram-apps/sdk-vue';
import AppLink from '@/components/AppLink.vue';
import AppPage from '@/components/AppPage.vue';
import AppDisplayData, { type DisplayDataRow } from '@/components/AppDisplayData.vue';

const initDataRef = useSignal(initData.state);
const initDataRaw = useSignal(initData.raw);
const canSendAfterDate = useSignal(initData.canSendAfterDate);

const initDataRows = computed<DisplayDataRow[] | undefined>(() => {
    if (!initDataRef.value || !initDataRaw.value) {
        return
    }

    const {
        authDate,
        hash,
        canSendAfter,
        queryId,
        startParam,
        chatType,
        chatInstance,
    } = initDataRef.value;

    return [
        { title: 'raw', value: initDataRaw.value },
        { title: 'auth_date', value: authDate.toLocaleString() },
        { title: 'auth_date (raw)', value: authDate.getTime() / 1000 },
        { title: 'hash', value: hash },
        { title: 'can_send_after', value: canSendAfterDate.value?.toISOString() },
        { title: 'can_send_after (raw)', value: canSendAfter },
        { title: 'query_id', value: queryId },
        { title: 'start_param', value: startParam },
        { title: 'chat_type', value: chatType },
        { title: 'chat_instance', value: chatInstance },
    ]
});

const userRows = computed<DisplayDataRow[] | undefined>(() => {
    const user = initDataRef.value?.user;

    return user ? getUserRows(user) : undefined;
})

const receiverRows = computed<DisplayDataRow[] | undefined>(() => {
    const receiver = initDataRef.value?.receiver;

    return receiver ? getUserRows(receiver) : undefined;
})

const chatRows = computed<DisplayDataRow[] | undefined>(() => {
    const chat = initDataRef.value?.chat;

    return chat
        ? [
            { title: 'id', value: chat.id.toString() },
            { title: 'title', value: chat.title },
            { title: 'type', value: chat.type },
            { title: 'username', value: chat.username },
            { title: 'photo_url', value: chat.photoUrl },
        ]
        : undefined;
})

function getUserRows(user: User): DisplayDataRow[] {
    return [
        { title: 'id', value: user.id.toString() },
        { title: 'username', value: user.username },
        { title: 'photo_url', value: user.photoUrl },
        { title: 'last_name', value: user.lastName },
        { title: 'first_name', value: user.firstName },
        { title: 'is_bot', value: user.isBot },
        { title: 'is_premium', value: user.isPremium },
        { title: 'language_code', value: user.languageCode },
        { title: 'allows_to_write_to_pm', value: user.allowsWriteToPm },
        { title: 'added_to_attachment_menu', value: user.addedToAttachmentMenu },
    ];
}
</script>

<template>
    <AppPage title="Init Data">
        <template #disclaimer>
            This page displays application
            <AppLink to="https://docs.telegram-mini-apps.com/platform/init-data">init data</AppLink>.
        </template>
        <i v-if="!initDataRows">Application was launched with missing init data</i>
        <template v-else>
            <div class="init-data-page__section">
                <h2 class="init-data-page__section-title">Init data</h2>
                <AppDisplayData :rows="initDataRows" />
            </div>

            <div class="init-data-page__section">
                <h2 class="init-data-page__section-title">User</h2>
                <i v-if="!userRows">User information missing</i>
                <AppDisplayData v-else :rows="userRows" />
            </div>

            <div class="init-data-page__section">
                <h2 class="init-data-page__section-title">Receiver</h2>
                <i v-if="!receiverRows">Receiver information missing</i>
                <AppDisplayData v-else :rows="receiverRows" />
            </div>

            <div class="init-data-page__section">
                <h2 class="init-data-page__section-title">Chat</h2>
                <i v-if="!chatRows">Chat information missing</i>
                <AppDisplayData v-else :rows="chatRows" />
            </div>
        </template>
    </AppPage>
</template>

<style scoped>
.init-data-page__section+.init-data-page__section {
    margin-top: 12px;
}

.init-data-page__section-title {
    margin-bottom: 4px;
}
</style>