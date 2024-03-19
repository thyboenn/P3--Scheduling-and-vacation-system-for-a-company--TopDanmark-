<script lang="ts">
  import {
    Autocomplete,
    Button,
    Card,
    Divider,
    Dropdown,
    DropdownShell,
    Label,
    Switch,
    TextField,
  } from "attractions";
  import { addDays, addMonths, addWeeks } from "date-fns";
  import { ChevronDownIcon, SaveIcon } from "svelte-feather-icons";
  import { maxRepeatAmounts, repeatTypes } from "../../lib/calendar";

  import { makeBackendRequestJson } from "../../lib/requestHelpers";
  import { convertDatesToStrings, convertStringsToDates } from "../../lib/types";
  import {
    ensureAssociates,
    ensureMe,
    getAssociateOptions,
  } from "../../stores/associate";
  import {
    addEventToStore,
    creating,
    stopCreatingEvent,
  } from "../../stores/calendar";
  import EventCard from "./EventCard.svelte";

  export let show: boolean;

  ensureAssociates();
  ensureMe();

  async function createEvent() {
    if (
      !$creating.extraOptions.shared &&
      !$creating.extraOptions.associateScheduleOptions.length
    ) {
      return;
    }

    const eventsToCreate = $creating.repeating ? $creating.repeatData.times : 1;

    for (let i = 0; i < eventsToCreate; i++) {
      const event = { ...$creating.newEvent };

      const addFunc = { Month: addMonths, Week: addWeeks, Day: addDays }[
        $creating.repeatData.type
      ];

      event.start = addFunc(event.start, $creating.repeatData.spacing * i);
      event.end = addFunc(event.end, $creating.repeatData.spacing * i);

      const associateId = $creating.extraOptions.associateScheduleOptions[0].id;
      const createdEvent = await ($creating.extraOptions.shared
        ? makeBackendRequestJson(
            "/Schedule/shared",
            "post",
            convertDatesToStrings(event, ["start", "end"])
          )
        : makeBackendRequestJson("/Schedule/associate", "post", {
            createEvent: convertDatesToStrings(event, ["start", "end"]),
            associateId,
          }));

      if (createdEvent) {
        const workEvent = convertStringsToDates(createdEvent, ["start", "end"]);
        addEventToStore(workEvent);
      }
    }
    stopCreatingEvent();
  }
</script>

<EventCard
  title="Opret begivenhed"
  event={$creating.newEvent}
  {show}
  on:stopEditing={stopCreatingEvent}
>
  <div slot="extra-content" class="spaced">
    <Card style="overflow:visible" class="spaced" outline>
      <div class="distribute-div">
        <Label class="attribute-label">Gentag</Label>
        <Switch bind:value={$creating.repeating} />
      </div>
      {#if $creating.repeating}
        <Divider />
        <Label>Hvor ofte</Label>
        <div class="distribute-div">
          <div class="row vertical-align-center">
            <!-- TODO: Anden løsning på mellemrum? -->
            <span>Hver&nbsp;</span>
            <TextField
              withItem
              itemRight
              noSpinner
              style="width: 4em"
              type="number"
              bind:value={$creating.repeatData.spacing}
              min={0}
              max={maxRepeatAmounts[$creating.repeatData.type]}
            >
              <span class="item">.</span>
            </TextField>
            <DropdownShell let:toggle>
              <Button on:click={toggle}>
                <span class="right-margin">
                  {repeatTypes[$creating.repeatData.type]}
                </span>
                <ChevronDownIcon />
              </Button>
              <Dropdown>
                <div class="dropdown-padding spacing">
                  <Button
                    on:click={() => {
                      $creating.repeatData.type = "Day";
                      toggle();
                    }}
                  >
                    {repeatTypes["Day"]}
                  </Button>
                  <Button
                    on:click={() => {
                      $creating.repeatData.type = "Week";
                      toggle();
                    }}
                  >
                    {repeatTypes["Week"]}
                  </Button>
                  <Button
                    on:click={() => {
                      $creating.repeatData.type = "Month";
                      toggle();
                    }}
                  >
                    {repeatTypes["Month"]}
                  </Button>
                </div>
              </Dropdown>
            </DropdownShell>
          </div>
        </div>
        <Label>Hvor mange gange</Label>
        <div class="distribute-div">
          <div class="distribute-div">
            <TextField
              style="width: 4em"
              noSpinner
              withItem
              itemRight
              type="number"
              min={0}
              max={52}
              bind:value={$creating.repeatData.times}
            >
              <span class="item">x</span>
            </TextField>
            <span>gange</span>
          </div>
        </div>
      {/if}
    </Card>
    <Card style="overflow: visible" class="spaced" outline>
      <div class="distribute-div">
        <Label class="attribute-label">Delt begivenhed</Label>
        <Switch bind:value={$creating.extraOptions.shared} />
      </div>
      {#if !$creating.extraOptions.shared}
        <Divider />
        <Label>Kalender</Label>
        <Autocomplete
          minSearchLength={1}
          maxOptions={1}
          getOptions={getAssociateOptions}
          bind:selection={$creating.extraOptions.associateScheduleOptions}
        />
      {/if}
    </Card>
  </div>
  <div slot="buttons" class="flex-row">
    <Button on:click={createEvent}>
      <SaveIcon />
      <span class="left-margin">Opret</span>
    </Button>
  </div>
</EventCard>

<style>
  .flex-row {
    display: flex;
    justify-content: space-between;
  }
  .distribute-div {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 0.5cm;
  }
</style>
