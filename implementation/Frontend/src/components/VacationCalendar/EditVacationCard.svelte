<script lang="ts">
  import {
    Button,
    DatePicker,
    Divider,
    Dropdown,
    DropdownShell,
    Label,
  } from "attractions";
  import {
    CheckIcon,
    ChevronDownIcon,
    Edit2Icon,
    SaveIcon,
    TrashIcon,
    XIcon,
  } from "svelte-feather-icons";

  import { convertStringsToDates, type VacationStatus } from "../../lib/types";
  import {
    handleDeletingVacation,
    handleEditingVacation,
    handleSettingStatus,
    statusTranslations,
  } from "../../lib/vacation";
  import {
    datesChanged,
    editing,
    stopEditingVacation,
    updateVacationInStore,
    vacationStatusChanged,
  } from "../../stores/vacation";
  import PopupCard from "../PopupCard.svelte";

  export let show: boolean;
  export let title: string;

  async function deleteVacation() {
    if ($editing.vacationId) {
      handleDeletingVacation($editing.vacationId);
      stopEditingVacation();
    }
  }
  async function saveChanges() {
    if ($editing.vacationId) {
      const editedVacationDto = await handleEditingVacation(
        $editing.vacationId,
        $editing.newPeriod
      );
      if (editedVacationDto) {
        const vacation = convertStringsToDates(editedVacationDto, ["start", "end"]);
        if ($vacationStatusChanged) {
          setVacationStatus($editing.status);
        }
        updateVacationInStore(vacation);
        stopEditingVacation();
      }
    }
  }
  async function setVacationStatus(status: VacationStatus) {
    if ($editing.vacationId) {
      $editing.status = status;
      const approvedVacation = await handleSettingStatus(
        $editing.vacationId,
        $editing.status
      );
      if (approvedVacation) {
        const vacation = convertStringsToDates(approvedVacation, ["start", "end"]);
        updateVacationInStore(vacation);
        stopEditingVacation();
      }
    }
  }
</script>

<PopupCard {title} {show} closeCallback={stopEditingVacation}>
  <svelte:fragment slot="title-icon">
    <Edit2Icon />
  </svelte:fragment>
  <div class="spaced">
    <DatePicker
      range
      bind:value={$editing.newPeriod}
      on:change={() => {
        $datesChanged = true;
      }}
    />

    <DropdownShell let:toggle>
      <div class="row space-between vertical-align-center">
        <Label>Status:</Label>
        <Button on:click={toggle}>
          <div
            style="align-items: center;  align-self: right"
            class="row space-between"
          >
            <span class="right-margin">
              {statusTranslations[$editing.status]}
            </span>
            <ChevronDownIcon />
          </div>
        </Button>
      </div>

      <Dropdown horizontalAlignment="end">
        <div class="dropdown-padding spacing">
          <Button
            on:click={() => {
              $editing.status = "Approved";
              $vacationStatusChanged = true;
              toggle();
            }}
          >
            <CheckIcon />
            <span class="left-margin">Godkend</span>
          </Button>
          <Button
            on:click={() => {
              $editing.status = "Denied";
              $vacationStatusChanged = true;
              toggle();
            }}
            danger
          >
            <XIcon />
            <span class="left-margin">Afvis</span>
          </Button>
        </div>
      </Dropdown>
    </DropdownShell>

    <Divider />
    <div class="row space-between">
      <Button on:click={deleteVacation} danger>
        <TrashIcon />
        <span class="left-margin">Slet</span>
      </Button>
      {#if $datesChanged || $vacationStatusChanged}
        <Button on:click={saveChanges}>
          <SaveIcon />
          <span class="left-margin">Gem</span>
        </Button>
      {/if}
    </div>
  </div>
</PopupCard>
