<script lang="ts">
  import { Button, DatePicker, Label, Switch } from "attractions";
  import { SunIcon } from "svelte-feather-icons";
  import { makeBackendRequestJson } from "../../lib/requestHelpers";

  import { convertDatesToStrings, convertStringsToDates } from "../../lib/types";
  import { me } from "../../stores/associate";
  import {
    addVacationToStore,
    creating,
    stopRequestingVacation,
  } from "../../stores/vacation";
  import PopupCard from "../PopupCard.svelte";

  export let show: boolean;
  export let title: string;

  async function createVacation() {
    if (!$creating.newVacation || !$me) return;

    const createdVacationDto = await makeBackendRequestJson(
      !$creating.isShared ? "/Vacation/request" : "/Vacation/force",
      "post",
      convertDatesToStrings($creating.newVacation, ["start", "end"])
    );

    if (createdVacationDto) {
      const vacation = convertStringsToDates(createdVacationDto, ["start", "end"]);
      addVacationToStore(vacation);
      stopRequestingVacation();
    }
  }
</script>

<PopupCard {title} {show} closeCallback={stopRequestingVacation}>
  <svelte:fragment slot="title-icon">
    <SunIcon />
  </svelte:fragment>

  <div class="spaced">
    <div>
      <Label>Periode</Label>
      <DatePicker range bind:value={$creating.newVacation} />
    </div>

    <div>
      <Label>Fælles ferie</Label>
      <Switch bind:value={$creating.isShared} />
    </div>

    <div class="row space-between">
      <Button on:click={createVacation} filled>
        {#if $creating.isShared}
          Opret ferie
        {:else}
          Ønsk ferie
        {/if}
      </Button>
      <Button on:click={stopRequestingVacation} danger outline>Annuller</Button>
    </div>
  </div>
</PopupCard>
